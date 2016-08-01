#region
using System;
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
		public bool isReadyForNewTimeLog = true;
		public bool WasNetworkAvailable = true;
		
		private const int MaxContinuousInterruptTime = 10; // minutes
		private const int RunawayTimerTime = 60; // one hour
		private Controller _controller;

		private OsTimerService _osTimerService;
		private int _savedLoggedTime;
		private int _savedInterruptTime;
		private Stopwatch _stopwatch = new Stopwatch();
		private String _taskId;
		private String _timeLogEntryId;
		


		// Events --------------------------------------------------------------------------------------------------------------------

		// Fired when network connection to the PDES changed from available to unavailable, or from unavailable to available
		// not real-time. We do network connections once per minute in TimeLoggingController
		// not represents global network availablility. Fired by and only by this TimeLoggingController.
		public event StateChangedEventHandler NetworkAvailabilityChanged;
		public void OnNetworkAvailabilityChanged(NetworkAvailabilityStates e, String message)
		{
			if (NetworkAvailabilityChanged != null)
			{
				NetworkAvailabilityChanged(this, new StateChangedEventArgs(e, message));
			}
		}

		// Fired when a new time log is created successfully, updated successfully/failed, or canceled by PDES.
		public event StateChangedEventHandler TimeLoggingStateChanged;
		public void OnTimeLoggingStateChanged(TimeLoggingControllerStates e, String message)
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
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			_controller = new Controller(service);
			_osTimerService = new OsTimerService(this);
		}

		public async System.Threading.Tasks.Task StopTiming()
		{
			_stopwatch.Stop();
			await Save();
		}


		public async System.Threading.Tasks.Task StartTiming(String _taskId)
		{

			Console.WriteLine("Trying to start timer for task: " + _taskId);

			await SetTaskId(_taskId);
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

		private async System.Threading.Tasks.Task SetTaskId(String newTaskId)
		{
			if (newTaskId.Equals(this._taskId))
			{
				return;
			}
			_stopwatch.Stop();
			await SaveIfNeeded();

			this._taskId = newTaskId;
			ReleaseTimeLogEntry(true);
		}

		public String GetTimingTaskId()
		{
			return IsTimerRunning() ? _taskId : null;
		}

		public Boolean IsTimerRunning()
		{
			return _stopwatch.IsRunning();
		}

		public String GetActiveTimeLogEntryId()
		{
			return _timeLogEntryId;
		}

		public async System.Threading.Tasks.Task SetLoggedTime(int minutes)
		{
			_stopwatch.SetLoggedMinutes(minutes);
			await Save();
		}

		public async System.Threading.Tasks.Task SetInterruptTime(int minutes)
		{
			_stopwatch.SetInterruptMinutes(minutes);
			await Save();
		}

		public async void Ping(Object stateInfo)
		{
			_stopwatch.MaybeCancelRunawayTimer(RunawayTimerTime);
			await Save();
		}

		private async System.Threading.Tasks.Task Save()
		{
			Console.WriteLine("save() called");
			try
			{
				await SaveIfNeeded();
				_osTimerService.SetBackgroundPingsEnabled(_stopwatch.IsRunning());

				if (!WasNetworkAvailable)
				{
					OnNetworkAvailabilityChanged(NetworkAvailabilityStates.BecameAvailable, String.Empty);
					WasNetworkAvailable = true;
				}
			}
			catch (CannotReachServerException e)
			{
				isReadyForNewTimeLog = false;

				_osTimerService.SetBackgroundPingsEnabled(true);

				OnTimeLoggingStateChanged(TimeLoggingControllerStates.TimeLogUpdateFailed, String.Empty);

				Console.WriteLine("save() catched CannotReachServerException. Background ping enabled.");

				if (WasNetworkAvailable)
				{
					OnNetworkAvailabilityChanged(NetworkAvailabilityStates.BecameUnavailable, String.Empty);
					WasNetworkAvailable = false;
				}
			}
		}

		private async System.Threading.Tasks.Task SaveIfNeeded()
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


						var value = await _controller.AddATimeLog(AccountStorage.DataSet, "", _stopwatch.Get_first_startTime().GetValueOrDefault(), _taskId, logged, interrupt, _stopwatch.IsRunning());

						_timeLogEntryId = "" + value.Id;

						Console.WriteLine("timelogentryid: " + _timeLogEntryId);
						_savedLoggedTime = logged;
						_savedInterruptTime = interrupt;

						OnTimeLoggingStateChanged(TimeLoggingControllerStates.TimeLogCreated, String.Empty);
					}
					catch (CancelTimeLoggingException e)
					{
						OnTimeLoggingStateChanged(TimeLoggingControllerStates.TimeLogCanceled, String.Empty);
						await HandleCancelTimeLoggingException(e);
					}
				}
			}
			else
			{
				if (_stopwatch.IsPaused() && _stopwatch.GetLoggedMinutes() < 0.5)
				{
					Console.WriteLine("Calling DeleteTimeLog(). Minutes: " + _stopwatch.GetTrailingLoggedMinutes());

					await _controller.DeleteTimeLog(AccountStorage.DataSet, _timeLogEntryId);
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

						await _controller.UpdateTimeLog(AccountStorage.DataSet, _timeLogEntryId, "", _stopwatch.Get_first_startTime(), _taskId,
						                               LoggedTimeDelta(), interruptTimeDelta(), _stopwatch.IsRunning());

						Console.WriteLine(">>>> timelog update: delta: " + logged + ", int: " + interrupt);

						_savedLoggedTime = logged;
						_savedInterruptTime = interrupt;
						OnTimeLoggingStateChanged(TimeLoggingControllerStates.TimeLogUpdated, "Time log updated");
					}
					catch (CancelTimeLoggingException e)
					{
						OnTimeLoggingStateChanged(TimeLoggingControllerStates.TimeLogCanceled, String.Empty);
						await HandleCancelTimeLoggingException(e);
					}
				}
			}
			isReadyForNewTimeLog = true;
		}

		private async System.Threading.Tasks.Task HandleCancelTimeLoggingException(CancelTimeLoggingException e)
		{
			Console.WriteLine("handleCancelTimeLoggingException() called");

			_stopwatch.CancelTimingAsOf(e.GetStopTime());
			await SaveIfNeeded();
			ReleaseTimeLogEntry(true);
		}

		private void ReleaseTimeLogEntry(Boolean resetStopwatch)
		{
			_timeLogEntryId = null;
			_savedLoggedTime = 0;
			_savedInterruptTime = 0;
			if (resetStopwatch)
			{
				_stopwatch.Reset();
			}
		}

		private Boolean TimeIsDiscrepant()
		{
			return LoggedTimeDelta() != 0 || interruptTimeDelta() != 0;
		}

		private int LoggedTimeDelta()
		{
			int ret = Round(_stopwatch.GetLoggedMinutes() - _savedLoggedTime);
			Console.WriteLine("loggedTimeDelta: " + ret);
			return ret;
		}

		private int interruptTimeDelta()
		{
			return Round(_stopwatch.GetInterruptMinutes() - _savedInterruptTime);
		}

		private int Round(double d)
		{
			return (int)Math.Round(d);
		}
	}
}

