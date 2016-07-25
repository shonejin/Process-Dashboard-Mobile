using System;
using System.Threading;
namespace ProcessDashboard
{
	public class OsTimerService
	{
		private Timer pingTimer = null;
		private TimeLoggingController controller;

		public OsTimerService(TimeLoggingController controller)
		{
			this.controller = controller;
		}

		public void setBackgroundPingsEnabled(Boolean enable)
		{
			// nothing changes
			if (enable == (pingTimer != null))
			{
				return;
			}
			// state change from stopped to started
			if (enable)
			{
				AutoResetEvent autoEvent = new AutoResetEvent(false);
				TimerCallback tcb = controller.ping;
				pingTimer = new System.Threading.Timer(tcb, autoEvent, 0, 1000 * 60);
			}
			// state change from started to stopped
			else
			{
				pingTimer.Dispose();
				pingTimer = null;
			}
		}
	}
}

