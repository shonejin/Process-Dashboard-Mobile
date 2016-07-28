using System;
using System.Threading;
namespace ProcessDashboard
{
	public class OsTimerService
	{
		private Timer _pingTimer = null;
		private TimeLoggingController _controller;

		public OsTimerService(TimeLoggingController controller)
		{
			this._controller = controller;
		}

		public void SetBackgroundPingsEnabled(Boolean enable)
		{
			// nothing changes
			if (enable == (_pingTimer != null))
			{
				return;
			}
			// state change from stopped to started
			if (enable)
			{
				AutoResetEvent autoEvent = new AutoResetEvent(false);
				TimerCallback tcb = _controller.Ping;
				_pingTimer = new System.Threading.Timer(tcb, autoEvent, 0, 1000 * 60);
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

