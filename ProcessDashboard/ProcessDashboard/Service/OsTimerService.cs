#region
using System.Threading;
#endregion
namespace ProcessDashboard
{
    public class OsTimerService
    {
        private TimeLoggingController _controller;
        private Timer _pingTimer;

        public OsTimerService(TimeLoggingController controller)
        {
            _controller = controller;
        }

        public void SetBackgroundPingsEnabled(bool enable)
        {
            // nothing changes
            if (enable == (_pingTimer != null))
            {
                return;
            }
            // state change from stopped to started
            if (enable)
            {
                var autoEvent = new AutoResetEvent(false);
                TimerCallback tcb = _controller.Ping;
                _pingTimer = new Timer(tcb, autoEvent, 0, 1000*60);
            }
            // state change from started to stopped
            else
            {
                _pingTimer.Dispose();
                _pingTimer = null;
            }
        }
    }
}