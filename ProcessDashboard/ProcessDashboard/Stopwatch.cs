using System;
namespace ProcessDashboard
{
	public class Stopwatch
	{
		private DateTime firstStartTime = new DateTime(0, DateTimeKind.Utc);
		private DateTime startTime = new DateTime(0, DateTimeKind.Utc);
		private DateTime stopTime = new DateTime(0, DateTimeKind.Utc);
		private long loggedMillis = 0;
		private long interruptMillis = 0;
		private const double MINUTES = 60000.0;

		public DateTime getFirstStartTime()
		{
			return firstStartTime;
		}

		public void start()
		{
			if (startTime.Ticks == 0)
			{
				startTime = DateTime.UtcNow;
				if (firstStartTime.Ticks == 0)
				{
					firstStartTime = startTime;
				}
				if (stopTime.Ticks != 0)
				{
					interruptMillis += (long)(startTime - stopTime).TotalMilliseconds;
				}
			}
		}

		public void stop()
		{
			stopAsOf(DateTime.UtcNow);
		}

		private void stopAsOf(DateTime when)
		{
			if (startTime.Ticks != 0)
			{
				stopTime = when;
				loggedMillis += (long)(stopTime - startTime).TotalMilliseconds;
				startTime = new DateTime(0, DateTimeKind.Utc);
			}
		}

		public bool isRunning()
		{
			return startTime.Ticks != 0;
		}

		public bool isPaused()
		{
			return startTime.Ticks == 0;
		}

		public void reset()
		{
			firstStartTime = new DateTime(0, DateTimeKind.Utc);
			startTime = new DateTime(0, DateTimeKind.Utc);
			stopTime = new DateTime(0, DateTimeKind.Utc);
			loggedMillis = 0;
			interruptMillis = 0;
		}

		public void setLoggedMinutes(double minutes)
		{
			if (isRunning())
			{
				stop();
				loggedMillis = (long)(minutes * MINUTES);
				start();
			}
			else
			{
				loggedMillis = (long)(minutes * MINUTES);
			}
		}

		public double getLoggedMinutes()
		{
			Console.WriteLine("stopwatch-getLoggedMinutes: loggedMillis=" + loggedMillis + "; isRunning=" + isRunning());
			long time = (long)(loggedMillis / MINUTES);
			if (isRunning())
			{
				time += (long)(DateTime.UtcNow - startTime).TotalMinutes;
			}
			Console.WriteLine("return: " + time);
			return time;
		}

		public void setInterruptMinutes(double minutes)
		{
			interruptMillis = (long)(minutes * MINUTES);
		}

		public double getInterruptMinutes()
		{
			return interruptMillis / MINUTES;
		}

		public double getTrailingLoggedMinutes()
		{
			if (isRunning())
			{
				Console.WriteLine("getTrailingLoggedMinutes: from " + startTime.ToString() + " has minutes: " + (DateTime.UtcNow - startTime).TotalMinutes);
				return (DateTime.UtcNow - startTime).TotalMinutes;
			}
			else
			{
				Console.WriteLine("getTrailingLoggedMinutes: not running. Return 0 ");
				return 0;
			}
		}

		public double getTrailingInterruptMinutes()
		{
			if (isPaused() && stopTime.Ticks != 0)
			{
				return (DateTime.UtcNow - stopTime).TotalMinutes;
			}
			else
			{
				return 0;
			}
		}

		public void maybeCancelRunawayTimer(double maxTrailingLoggedMinutes)
		{
			double trailingLoggedMinutes = getTrailingLoggedMinutes();
			if (trailingLoggedMinutes < maxTrailingLoggedMinutes)
			{
				return;
			}
			double minutesToCancel = trailingLoggedMinutes - maxTrailingLoggedMinutes;
			long millisToCancel = (long)(minutesToCancel * MINUTES);
			DateTime cancelTime = DateTime.UtcNow.AddMilliseconds(-millisToCancel);
			cancelTimingAsOf(cancelTime);
		}

		public void cancelTimingAsOf(DateTime cancellationTime)
		{
			DateTime now = DateTime.UtcNow;
			if (cancellationTime > now)
			{
				cancellationTime = now;
			}

			if (startTime.Ticks != 0)
			{
				if (startTime < cancellationTime)
				{
					stopAsOf(cancellationTime);
					return;
				}
				else
				{
					if (stopTime.Ticks != 0)
					{
						long interrupToDiscard = (long)(startTime - stopTime).TotalMilliseconds;
						interruptMillis -= interrupToDiscard;
						if (interruptMillis < 0)
						{
							interruptMillis = 0;
						}
					}
					startTime = new DateTime(0, DateTimeKind.Utc);
				}
			}

			if (stopTime.Ticks != 0 && cancellationTime < stopTime)
			{
				long overlapMillis = (long)(stopTime - cancellationTime).TotalMilliseconds;
				if (overlapMillis > loggedMillis)
				{
					reset();
				}
				else
				{
					loggedMillis = loggedMillis - overlapMillis;
					stopTime = cancellationTime;
				}
			}
		}
	}
}
