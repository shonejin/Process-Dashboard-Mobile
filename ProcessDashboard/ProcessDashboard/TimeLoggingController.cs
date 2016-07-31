#region
using System;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;
#endregion
namespace ProcessDashboard
{
    public class TimeLoggingController
    {
        private const int MaxContinuousInterruptTime = 10; // minutes
        private const int RunawayTimerTime = 60; // one hour
        private Controller _controller;

        private OsTimerService _osTimerService;
        private int _savedInterruptTime;
        private int _savedLoggedTime;
        private Stopwatch _stopwatch = new Stopwatch();
        private string _taskId;
        private string _timeLogEntryId;

        public TimeLoggingController()
        {
            var apiService = new ApiTypes();
            var service = new PDashServices(apiService);
            _controller = new Controller(service);
            _osTimerService = new OsTimerService(this);
        }

        // TODO: throws CannotReachServerException
        public void StartTiming(string taskId)
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
        private void SetTaskId(string newTaskId)
        {
            if (newTaskId.Equals(_taskId))
            {
                return;
            }
            _stopwatch.Stop();
            SaveIfNeeded();

            _taskId = newTaskId;
            ReleaseTimeLogEntry(true);
        }

        public void StopTiming()
        {
            _stopwatch.Stop();
            Save();
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

        public void Ping(object stateInfo)
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
            catch (CannotReachServerException)
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
                        var logged = Round(_stopwatch.GetLoggedMinutes());
                        var interrupt = Round(_stopwatch.GetInterruptMinutes());

                        var value = await _controller.AddATimeLog(Settings.GetInstance().Dataset, "",
                            _stopwatch.GetFirstStartTime(), _taskId, logged, interrupt, _stopwatch.IsRunning());
                        _timeLogEntryId = "" + value.Id;

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
                    await _controller.DeleteTimeLog(Settings.GetInstance().Dataset, _timeLogEntryId);
                    ReleaseTimeLogEntry(false);
                }
                else if (TimeIsDiscrepant())
                {
                    try
                    {
                        var logged = Round(_stopwatch.GetLoggedMinutes());
                        var interrupt = Round(_stopwatch.GetInterruptMinutes());
                        await
                            _controller.UpdateTimeLog(Settings.GetInstance().Dataset, _timeLogEntryId, "",
                                _stopwatch.GetFirstStartTime(), _taskId,
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
            return Round(_stopwatch.GetLoggedMinutes() - _savedLoggedTime);
        }

        private int InterruptTimeDelta()
        {
            return Round(_stopwatch.GetInterruptMinutes() - _savedInterruptTime);
        }

        private int Round(double d)
        {
            return (int) Math.Round(d);
        }
    }
}