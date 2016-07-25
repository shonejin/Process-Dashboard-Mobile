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

		// TODO: throws CannotReachServerException
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

		public async System.Threading.Tasks.Task stopTiming()
		{
			stopwatch.stop();
			await save();
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

		public void ping(Object stateInfo)
		{
			stopwatch.maybeCancelRunawayTimer(runawayTimerTime);
			save();
		}

		private async System.Threading.Tasks.Task save()
		{
			Console.WriteLine("save() called");
			try
			{
				await saveIfNeeded();
				osTimerService.setBackgroundPingsEnabled(stopwatch.isRunning());
			}
			catch (CannotReachServerException e)
			{
				osTimerService.setBackgroundPingsEnabled(true);
				Console.WriteLine("save() catched CannotReachServerException. Background ping enabled.");
			}
		}

		// TODO: throws CannotReachServerException
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

						try
						{
							var value = await controller.AddATimeLog(Settings.GetInstance().Dataset, "",
												 "" + stopwatch.getFirstStartTime(), taskId, logged, interrupt, stopwatch.isRunning());

							timeLogEntryId = "" + value.TimeLogEntry.Id;
						}
						catch (Exception e)
						{
							Console.WriteLine(e.ToString());
							throw e;
						}


						Console.WriteLine("timelogentryid: " + timeLogEntryId);
                       
                        savedLoggedTime = logged;
						savedInterruptTime = interrupt;
					}
					catch (CancelTimeLoggingException e)
					{
						await handleCancelTimeLoggingException(e);
					}
				}
			}
			else
			{
				if (stopwatch.isPaused() && stopwatch.getTrailingLoggedMinutes() < 0.5)
				{
					Console.WriteLine("Calling DeleteTimeLog(). Minutes: " + stopwatch.getTrailingLoggedMinutes());

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
						await controller.UpdateTimeLog(Settings.GetInstance().Dataset,timeLogEntryId,"", stopwatch.getFirstStartTime().ToString(Settings.GetInstance().DateTimePattern),taskId,
                            loggedTimeDelta(), interruptTimeDelta(),
							stopwatch.isRunning());
						savedLoggedTime = logged;
						savedInterruptTime = interrupt;
					}
					catch (CancelTimeLoggingException e)
					{
						//TODO: update UI
						await handleCancelTimeLoggingException(e);
					}
				}
			}
		}

		// TODO: CannotReachServerException
		private async System.Threading.Tasks.Task handleCancelTimeLoggingException(CancelTimeLoggingException e)
		{
			Console.WriteLine("handleCancelTimeLoggingException() called");

			stopwatch.cancelTimingAsOf(e.getStopTime());
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

