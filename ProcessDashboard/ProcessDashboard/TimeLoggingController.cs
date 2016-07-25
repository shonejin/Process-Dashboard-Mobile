using System;
using System.Threading;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;

namespace ProcessDashboard
{
	public class TimeLoggingController
	{
		private const int maxContinuousInterruptTime = 10; // minutes
		private const int runawayTimerTime = 60; // one hour
		private String taskId;
		private Stopwatch stopwatch = new Stopwatch();
		private String timeLogEntryId;
		private int savedLoggedTime;
		private int savedInterruptTime;
		private Controller controller;

		private OsTimerService osTimerService;

		public TimeLoggingController()
		{
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			controller = new Controller(service);
			osTimerService = new OsTimerService(this);
		}

		// TODO: throws CannotReachServerException
		public void startTiming(String taskId)
		{

			Console.WriteLine("Trying to start timer for task: " + taskId);

			setTaskId(taskId);
			if (stopwatch.getTrailingLoggedMinutes() > maxContinuousInterruptTime)
			{
				saveIfNeeded();
				releaseTimeLogEntry(true);
			}
			stopwatch.start();
			save();

			if (stopwatch.isPaused())
			{
				stopwatch.start();
				save();
			}
		}

		// TODO: throws CannotReachServerException
		private void setTaskId(String newTaskId)
		{
			if (newTaskId.Equals(this.taskId))
			{
				return;
			}
			stopwatch.stop();
			saveIfNeeded();

			this.taskId = newTaskId;
			releaseTimeLogEntry(true);
		}

		public void stopTiming()
		{
			stopwatch.stop();
			save();
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

		public void setLoggedTime(int minutes)
		{
			stopwatch.setLoggedMinutes(minutes);
			save();
		}

		public void setInterruptTime(int minutes)
		{
			stopwatch.setInterruptMinutes(minutes);
			save();
		}

		public void ping(Object stateInfo)
		{
			stopwatch.maybeCancelRunawayTimer(runawayTimerTime);
			save();
		}

		private void save()
		{
			Console.WriteLine("save() called");
			try
			{
				saveIfNeeded();
				osTimerService.setBackgroundPingsEnabled(stopwatch.isRunning());
			}
			catch (CannotReachServerException e)
			{
				osTimerService.setBackgroundPingsEnabled(true);
				Console.WriteLine("save() catched CannotReachServerException. Background ping enabled.");
			}
		}

		// TODO: throws CannotReachServerException
		private async void saveIfNeeded()
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
						timeLogEntryId = ""+controller.AddATimeLog(Settings.GetInstance().Dataset,"", ""+stopwatch.getFirstStartTime(), taskId, logged, interrupt, stopwatch.isRunning()).Id;

                       
                        savedLoggedTime = logged;
						savedInterruptTime = interrupt;
					}
					catch (CancelTimeLoggingException e)
					{
						handleCancelTimeLoggingException(e);
					}
				}
			}
			else
			{
				if (stopwatch.isPaused() && stopwatch.getTrailingLoggedMinutes() < 0.5)
				{
					Console.WriteLine("Calling DeleteTimeLog()");

					await controller.DeleteTimeLog(Settings.GetInstance().Dataset,timeLogEntryId);
					releaseTimeLogEntry(false);
				}
				else if (timeIsDiscrepant())
				{
					try
					{
						int logged = round(stopwatch.getLoggedMinutes());
						int interrupt = round(stopwatch.getInterruptMinutes());

						Console.WriteLine("Calling UpdateTimeLog()");
						await controller.UpdateTimeLog(Settings.GetInstance().Dataset,timeLogEntryId,"", stopwatch.getFirstStartTime().ToString(),taskId,

                            loggedTimeDelta(), interruptTimeDelta(),
							stopwatch.isRunning());
						savedLoggedTime = logged;
						savedInterruptTime = interrupt;
					}
					catch (CancelTimeLoggingException e)
					{
						//TODO: update UI
						handleCancelTimeLoggingException(e);
					}
				}
			}
		}

		// TODO: CannotReachServerException
		private void handleCancelTimeLoggingException(CancelTimeLoggingException e)
		{
			Console.WriteLine("handleCancelTimeLoggingException() called");

			stopwatch.cancelTimingAsOf(e.getStopTime());
			saveIfNeeded();
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
			return round(stopwatch.getLoggedMinutes() - savedLoggedTime);
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

