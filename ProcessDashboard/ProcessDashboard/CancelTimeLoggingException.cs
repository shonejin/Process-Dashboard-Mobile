using System;
namespace ProcessDashboard
{
	public class CancelTimeLoggingException : Exception
	{
		private DateTime stopTime;

		public CancelTimeLoggingException(DateTime stopTime)
		{
			this.stopTime = stopTime;
		}

		public DateTime getStopTime()
		{
			return stopTime;
		}
	}
}