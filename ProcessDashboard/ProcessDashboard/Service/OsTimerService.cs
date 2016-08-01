#region
using System.Threading;
using System;
#endregion
namespace ProcessDashboard
{
	public class OsTimerService
	{
		private TimeLoggingController _controller;
		private Timer _pingTimer = null;
		
		public OsTimerService(TimeLoggingController _controller)
		{
			this._controller = _controller;
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