#region
using System;
using System.Threading.Tasks;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;
#endregion
namespace ProcessDashboard
{

	// delegate used for state changes of Process Dashboard.
	// use this delegate to trigger UI updates
	public delegate void StateChangedEventHandler(object sender, StateChangedEventArgs e);

	public class TimeLoggingController
	{
		public bool IsReadyForNewTimeLog = true;
		public bool WasNetworkAvailable = true;
		
		private const int MaxContinuousInterruptTime = 10; // minutes
		private const int RunawayTimerTime = 60; // one hour
		private Controller _controller;

		private OsTimerService _osTimerService;
		private int _savedLoggedTime;
		private int _savedInterruptTime;
		private Stopwatch _stopwatch = new Stopwatch();
		private string _taskId;
		private string _timeLogEntryId;
		


		// Events --------------------------------------------------------------------------------------------------------------------

		// Fired when network connection to the PDES changed from available to unavailable, or from unavailable to available
		// not real-time. We do network connections once per minute in TimeLoggingController
		// not represents global network availablility. Fired by and only by this TimeLoggingController.
		public event StateChangedEventHandler NetworkAvailabilityChanged;
		public void OnNetworkAvailabilityChanged(NetworkAvailabilityStates e, string message)
		{
			if (NetworkAvailabilityChanged != null)
			{
				NetworkAvailabilityChanged(this, new StateChangedEventArgs(e, message));
			}
		}

		// Fired when a new time log is created successfully, updated successfully/failed, or canceled by PDES.
		public event StateChangedEventHandler TimeLoggingStateChanged;
		public void OnTimeLoggingStateChanged(TimeLoggingControllerStates e, string message)
		{
			if (TimeLoggingStateChanged != null)
			{
				TimeLoggingStateChanged(this, new StateChangedEventArgs(e, message));
			}
		}

		// -------------------------------------------------------------------------------------------------------------------- Events

		// this class is a singleton
		private static TimeLoggingController _instance;
		public static TimeLoggingController GetInstance()
		{
			return _instance ?? (_instance = new TimeLoggingController());
		}

		public TimeLoggingController()
		{
			var apiService = new ApiTypes();
			var service = new PDashServices(apiService);
			_controller = new Controller(service);
			_osTimerService = new OsTimerService(this);
		}

		public async Task StopTiming()
		{
			_stopwatch.Stop();
			await Save();
		}


		public async Task StartTiming(string taskId)
		{

			Console.WriteLine("Trying to start timer for task: " + taskId);

			await SetTaskId(taskId);
			if (_stopwatch.GetTrailingLoggedMinutes() > MaxContinuousInterruptTime)
			{
				await SaveIfNeeded();
				ReleaseTimeLogEntry(true);
			}

			_stopwatch.Start();
			await Save();

			if (_stopwatch.IsPaused())
			{
				_stopwatch.Start();
				await Save();
			}
		}

		private async Task SetTaskId(string newTaskId)
		{
			if (newTaskId.Equals(_taskId))
			{
				return;
			}
			_stopwatch.Stop();
			await SaveIfNeeded();

			_taskId = newTaskId;
			ReleaseTimeLogEntry(true);
		}

		public string GetTimingTaskId()
		{
			return IsTimerRunning() ? _taskId : null;
		}

		public bool IsTimerRunning()
		{
			return _stopwatch.IsRunning();
		}

		public string GetActiveTimeLogEntryId()
		{
			return _timeLogEntryId;
		}

		public async Task SetLoggedTime(int minutes)
		{
			_stopwatch.SetLoggedMinutes(minutes);
			await Save();
		}

		public async Task SetInterruptTime(int minutes)
		{
			_stopwatch.SetInterruptMinutes(minutes);
			await Save();
		}

		public async void Ping(object stateInfo)
		{
			_stopwatch.MaybeCancelRunawayTimer(RunawayTimerTime);
			await Save();
		}

		private async Task Save()
		{
			Console.WriteLine("save() called");
			try
			{
				await SaveIfNeeded();
				_osTimerService.SetBackgroundPingsEnabled(_stopwatch.IsRunning());

				if (!WasNetworkAvailable)
				{
					OnNetworkAvailabilityChanged(NetworkAvailabilityStates.BecameAvailable, string.Empty);
					WasNetworkAvailable = true;
				}
			}
			catch (CannotReachServerException)
			{
				IsReadyForNewTimeLog = false;

				_osTimerService.SetBackgroundPingsEnabled(true);

				OnTimeLoggingStateChanged(TimeLoggingControllerStates.TimeLogUpdateFailed, string.Empty);

				Console.WriteLine("save() catched CannotReachServerException. Background ping enabled.");

				if (WasNetworkAvailable)
				{
					OnNetworkAvailabilityChanged(NetworkAvailabilityStates.BecameUnavailable, string.Empty);
					WasNetworkAvailable = false;
				}
			}
		}

		private async Task SaveIfNeeded()
		{
			Console.WriteLine("saveIfNeeded() called");

			if (_timeLogEntryId == null)
			{
				if (_stopwatch.IsRunning() || TimeIsDiscrepant())
				{
					try
					{
						int logged = Round(_stopwatch.GetLoggedMinutes());
						int interrupt = Round(_stopwatch.GetInterruptMinutes());


						var value = await _controller.AddATimeLog(Settings.GetInstance().Dataset, "", _stopwatch.Get_first_startTime(), _taskId, logged, interrupt, _stopwatch.IsRunning());

						_timeLogEntryId = "" + value.Id;

						Console.WriteLine("timelogentryid: " + _timeLogEntryId);
						_savedLoggedTime = logged;
						_savedInterruptTime = interrupt;

						OnTimeLoggingStateChanged(TimeLoggingControllerStates.TimeLogCreated, string.Empty);
					}
					catch (CancelTimeLoggingException e)
					{
						OnTimeLoggingStateChanged(TimeLoggingControllerStates.TimeLogCanceled, string.Empty);
						await HandleCancelTimeLoggingException(e);
					}
				}
			}
			else
			{
				if (_stopwatch.IsPaused() && _stopwatch.GetLoggedMinutes() < 0.5)
				{
					Console.WriteLine("Calling DeleteTimeLog(). Minutes: " + _stopwatch.GetTrailingLoggedMinutes());

					await _controller.DeleteTimeLog(Settings.GetInstance().Dataset, _timeLogEntryId);
					ReleaseTimeLogEntry(false);

					OnTimeLoggingStateChanged(TimeLoggingControllerStates.TimeLogUpdated, "Time log deleted because it is too short.");
				}
				else if (TimeIsDiscrepant())
				{
					try
					{
						int logged = Round(_stopwatch.GetLoggedMinutes());
						int interrupt = Round(_stopwatch.GetInterruptMinutes());

						Console.WriteLine("Calling UpdateTimeLog()");

						await _controller.UpdateTimeLog(Settings.GetInstance().Dataset, _timeLogEntryId, "", _stopwatch.Get_first_startTime(), _taskId,
						                               LoggedTimeDelta(), InterruptTimeDelta(), _stopwatch.IsRunning());

						Console.WriteLine(">>>> timelog update: delta: " + logged + ", int: " + interrupt);

						_savedLoggedTime = logged;
						_savedInterruptTime = interrupt;
						OnTimeLoggingStateChanged(TimeLoggingControllerStates.TimeLogUpdated, "Time log updated");
					}
					catch (CancelTimeLoggingException e)
					{
						OnTimeLoggingStateChanged(TimeLoggingControllerStates.TimeLogCanceled, string.Empty);
						await HandleCancelTimeLoggingException(e);
					}
				}
			}
			IsReadyForNewTimeLog = true;
		}

		private async Task HandleCancelTimeLoggingException(CancelTimeLoggingException e)
		{
			Console.WriteLine("handleCancelTimeLoggingException() called");

			_stopwatch.CancelTimingAsOf(e.GetStopTime());
			await SaveIfNeeded();
			ReleaseTimeLogEntry(true);
		}

		private void ReleaseTimeLogEntry(bool resetStopwatch)
		{
			_timeLogEntryId = null;
			_savedLoggedTime = 0;
			_savedInterruptTime = 0;
			if (resetStopwatch)
			{
				_stopwatch.Reset();
			}
		}

		private bool TimeIsDiscrepant()
		{
			return LoggedTimeDelta() != 0 || InterruptTimeDelta() != 0;
		}

		private int LoggedTimeDelta()
		{
			int ret = Round(_stopwatch.GetLoggedMinutes() - _savedLoggedTime);
			Console.WriteLine("loggedTimeDelta: " + ret);
			return ret;
		}

		private int InterruptTimeDelta()
		{
			return Round(_stopwatch.GetInterruptMinutes() - _savedInterruptTime);
		}

		private int Round(double d)
		{
			return (int)Math.Round(d);
		}
	}
}

