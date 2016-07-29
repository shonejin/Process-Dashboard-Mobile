using System;
using System.Threading;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;

namespace ProcessDashboard
{
	// delegate used for state changes of Process Dashboard.
	// use this delegate to trigger UI updates
	public delegate void StateChangedEventHandler(object sender, StateChangedEventArgs e);

	public class TimeLoggingController
	{

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

		public bool isReadyForNewTimeLog = true;

		private const int maxContinuousInterruptTime = 10; // minutes
		private const int runawayTimerTime = 60; // one hour
		private int savedLoggedTime;
		private int savedInterruptTime;

		private String taskId;
		private String timeLogEntryId;

		private Stopwatch stopwatch = new Stopwatch();
		private OsTimerService osTimerService;
		private Controller controller;

		public bool wasNetworkAvailable = true;

		public TimeLoggingController()
		{
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			controller = new Controller(service);
			osTimerService = new OsTimerService(this);
		}

		public async System.Threading.Tasks.Task stopTiming()
		{
			stopwatch.stop();
			await save();
		}


		public async System.Threading.Tasks.Task startTiming(String taskId)
		{

			Console.WriteLine("Trying to start timer for task: " + taskId);

			await setTaskId(taskId);
			if (stopwatch.getTrailingLoggedMinutes() > maxContinuousInterruptTime)
			{
				await saveIfNeeded();
				releaseTimeLogEntry(true);
			}

			stopwatch.start();
			await save();

			if (stopwatch.isPaused())
			{
				stopwatch.start();
				await save();
			}
		}

		private async System.Threading.Tasks.Task setTaskId(String newTaskId)
		{
			if (newTaskId.Equals(this.taskId))
			{
				return;
			}
			stopwatch.stop();
			await saveIfNeeded();

			this.taskId = newTaskId;
			releaseTimeLogEntry(true);
		}

		public String getTimingTaskId()
		{
			return isTimerRunning() ? taskId : null;
		}

		public Boolean isTimerRunning()
		{
			return stopwatch.isRunning();
		}

		public String getActiveTimeLogEntryId()
		{
			return timeLogEntryId;
		}

		public async System.Threading.Tasks.Task setLoggedTime(int minutes)
		{
			stopwatch.setLoggedMinutes(minutes);
			await save();
		}

		public async System.Threading.Tasks.Task setInterruptTime(int minutes)
		{
			stopwatch.setInterruptMinutes(minutes);
			await save();
		}

		public async void ping(Object stateInfo)
		{
			stopwatch.maybeCancelRunawayTimer(runawayTimerTime);
			await save();
		}

		private async System.Threading.Tasks.Task save()
		{
			Console.WriteLine("save() called");
			try
			{
				await saveIfNeeded();
				osTimerService.setBackgroundPingsEnabled(stopwatch.isRunning());

				if (!wasNetworkAvailable)
				{
					OnNetworkAvailabilityChanged(NetworkAvailabilityStates.BecameAvailable, String.Empty);
					wasNetworkAvailable = true;
				}
			}
			catch (CannotReachServerException e)
			{
				isReadyForNewTimeLog = false;

				osTimerService.setBackgroundPingsEnabled(true);

				OnTimeLoggingStateChanged(TimeLoggingControllerStates.TimeLogUpdateFailed, String.Empty);

				Console.WriteLine("save() catched CannotReachServerException. Background ping enabled.");

				if (wasNetworkAvailable)
				{
					OnNetworkAvailabilityChanged(NetworkAvailabilityStates.BecameUnavailable, String.Empty);
					wasNetworkAvailable = false;
				}
			}
		}

		private async System.Threading.Tasks.Task saveIfNeeded()
		{
			Console.WriteLine("saveIfNeeded() called");

			if (timeLogEntryId == null)
			{
				if (stopwatch.isRunning() || timeIsDiscrepant())
				{
					try
					{
						int logged = round(stopwatch.getLoggedMinutes());
						int interrupt = round(stopwatch.getInterruptMinutes());


						var value = await controller.AddATimeLog(Settings.GetInstance().Dataset, "", "" + stopwatch.getFirstStartTime(), taskId, logged, interrupt, stopwatch.isRunning());

						timeLogEntryId = "" + value.TimeLogEntry.Id;

						Console.WriteLine("timelogentryid: " + timeLogEntryId);
						savedLoggedTime = logged;
						savedInterruptTime = interrupt;

						OnTimeLoggingStateChanged(TimeLoggingControllerStates.TimeLogCreated, String.Empty);
					}
					catch (CancelTimeLoggingException e)
					{
						OnTimeLoggingStateChanged(TimeLoggingControllerStates.TimeLogCanceled, String.Empty);
						await handleCancelTimeLoggingException(e);
					}
				}
			}
			else
			{
				if (stopwatch.isPaused() && stopwatch.getLoggedMinutes() < 0.5)
				{
					Console.WriteLine("Calling DeleteTimeLog(). Minutes: " + stopwatch.getTrailingLoggedMinutes());

					await controller.DeleteTimeLog(Settings.GetInstance().Dataset, timeLogEntryId);
					releaseTimeLogEntry(false);

					OnTimeLoggingStateChanged(TimeLoggingControllerStates.TimeLogUpdated, "Time log deleted because it is too short.");
				}
				else if (timeIsDiscrepant())
				{
					try
					{
						int logged = round(stopwatch.getLoggedMinutes());
						int interrupt = round(stopwatch.getInterruptMinutes());

						Console.WriteLine("Calling UpdateTimeLog()");

						await controller.UpdateTimeLog(Settings.GetInstance().Dataset, timeLogEntryId, "comment", stopwatch.getFirstStartTime().ToString(Settings.GetInstance().DateTimePattern), taskId,
						                               loggedTimeDelta(), interruptTimeDelta(), stopwatch.isRunning());

						Console.WriteLine(">>>> timelog update: delta: " + logged + ", int: " + interrupt);

						savedLoggedTime = logged;
						savedInterruptTime = interrupt;
						OnTimeLoggingStateChanged(TimeLoggingControllerStates.TimeLogUpdated, "Time log updated");
					}
					catch (CancelTimeLoggingException e)
					{
						OnTimeLoggingStateChanged(TimeLoggingControllerStates.TimeLogCanceled, String.Empty);
						await handleCancelTimeLoggingException(e);
					}
				}
			}
			isReadyForNewTimeLog = true;
		}

		// TODO: CannotReachServerException
		private async System.Threading.Tasks.Task handleCancelTimeLoggingException(CancelTimeLoggingException e)
		{
			Console.WriteLine("handleCancelTimeLoggingException() called");

			stopwatch.cancelTimingAsOf(e.GetStopTime());
			await saveIfNeeded();
			releaseTimeLogEntry(true);
		}

		private void releaseTimeLogEntry(Boolean resetStopwatch)
		{
			timeLogEntryId = null;
			savedLoggedTime = 0;
			savedInterruptTime = 0;
			if (resetStopwatch)
			{
				stopwatch.reset();
			}
		}

		private Boolean timeIsDiscrepant()
		{
			return loggedTimeDelta() != 0 || interruptTimeDelta() != 0;
		}

		private int loggedTimeDelta()
		{
			int ret = round(stopwatch.getLoggedMinutes() - savedLoggedTime);
			Console.WriteLine("loggedTimeDelta: " + ret);
			return ret;
		}

		private int interruptTimeDelta()
		{
			return round(stopwatch.getInterruptMinutes() - savedInterruptTime);
		}

		private int round(double d)
		{
			return (int)Math.Round(d);
		}
	}
}
