using System;
using System.Threading;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;

namespace ProcessDashboard
{
	public class TimeLoggingController
	{
		private const int MaxContinuousInterruptTime = 10; // minutes
		private const int RunawayTimerTime = 60; // one hour
		private String _taskId;
		private Stopwatch _stopwatch = new Stopwatch();
		private String _timeLogEntryId;
		private int _savedLoggedTime;
		private int _savedInterruptTime;
		private Controller _controller;

		private OsTimerService _osTimerService;

		public TimeLoggingController()
		{
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			_controller = new Controller(service);
			_osTimerService = new OsTimerService(this);
		}

		// TODO: throws CannotReachServerException
		public void StartTiming(String taskId)
		{
			SetTaskId(taskId);
			if (_stopwatch.GetTrailingLoggedMinutes() > MaxContinuousInterruptTime)
			{
				SaveIfNeeded();
				ReleaseTimeLogEntry(true);
			}
			_stopwatch.Start();
			Save();

			if (_stopwatch.IsPaused())
			{
				_stopwatch.Start();
				Save();
			}
		}

		// TODO: throws CannotReachServerException
		private void SetTaskId(String newTaskId)
		{
			if (newTaskId.Equals(this._taskId))
			{
				return;
			}
			_stopwatch.Stop();
			SaveIfNeeded();

			this._taskId = newTaskId;
			ReleaseTimeLogEntry(true);
		}

		public void StopTiming()
		{
			_stopwatch.Stop();
			Save();
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

		public void SetLoggedTime(int minutes)
		{
			_stopwatch.SetLoggedMinutes(minutes);
			Save();
		}

		public void SetInterruptTime(int minutes)
		{
			_stopwatch.SetInterruptMinutes(minutes);
			Save();
		}

		public void Ping(Object stateInfo)
		{
			_stopwatch.MaybeCancelRunawayTimer(RunawayTimerTime);
			Save();
		}

		private void Save()
		{
			try
			{
				SaveIfNeeded();
				_osTimerService.SetBackgroundPingsEnabled(_stopwatch.IsRunning());
			}
			catch (CannotReachServerException e)
			{
				_osTimerService.SetBackgroundPingsEnabled(true);
			}
		}

		// TODO: throws CannotReachServerException
		private async void SaveIfNeeded()
		{
			if (_timeLogEntryId == null)
			{
				if (_stopwatch.IsRunning() || TimeIsDiscrepant())
				{
					try
					{
						int logged = Round(_stopwatch.GetLoggedMinutes());
						int interrupt = Round(_stopwatch.GetInterruptMinutes());
					    
					    var value = await _controller.AddATimeLog(Settings.GetInstance().Dataset, "",
					                             "" + _stopwatch.GetFirstStartTime(), _taskId, logged, interrupt, _stopwatch.IsRunning());
                        _timeLogEntryId = ""+ value.TimeLogEntry.Id;

                        _savedLoggedTime = logged;
						_savedInterruptTime = interrupt;
					}
					catch (CancelTimeLoggingException e)
					{
						HandleCancelTimeLoggingException(e);
					}
				}
			}
			else
			{
				if (_stopwatch.IsPaused() && _stopwatch.GetTrailingLoggedMinutes() < 0.5)
				{
					await _controller.DeleteTimeLog(Settings.GetInstance().Dataset,_timeLogEntryId);
					ReleaseTimeLogEntry(false);
				}
				else if (TimeIsDiscrepant())
				{
					try
					{
						int logged = Round(_stopwatch.GetLoggedMinutes());
						int interrupt = Round(_stopwatch.GetInterruptMinutes());
						await _controller.UpdateTimeLog(Settings.GetInstance().Dataset,_timeLogEntryId,"", _stopwatch.GetFirstStartTime().ToString(),_taskId,

                            LoggedTimeDelta(), InterruptTimeDelta(),
							_stopwatch.IsRunning());
						_savedLoggedTime = logged;
						_savedInterruptTime = interrupt;
					}
					catch (CancelTimeLoggingException e)
					{
						HandleCancelTimeLoggingException(e);
					}
				}
			}
		}

		// TODO: CannotReachServerException
		private void HandleCancelTimeLoggingException(CancelTimeLoggingException e)
		{
			_stopwatch.CancelTimingAsOf(e.GetStopTime());
			SaveIfNeeded();
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
			return LoggedTimeDelta() != 0 || InterruptTimeDelta() != 0;
		}

		private int LoggedTimeDelta()
		{
			return Round(_stopwatch.GetLoggedMinutes() - _savedLoggedTime);
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

