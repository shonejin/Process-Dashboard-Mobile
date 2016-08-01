#region
using System;
#endregion

namespace ProcessDashboard
{
	public class Stopwatch
	{
		private const double MINUTES = 60000.0;
		private DateTime _first_startTime = new DateTime(0, DateTimeKind.Utc);
		private long _interruptMillis = 0;
		private long _loggedMillis = 0;
		private DateTime _startTime = new DateTime(0, DateTimeKind.Utc);
		private DateTime _stopTime = new DateTime(0, DateTimeKind.Utc);
		
		public DateTime Get_first_startTime()
		{
			return _first_startTime;
		}

		public void Start()
		{
			if (_startTime.Ticks == 0)
			{
				_startTime = DateTime.UtcNow;
				if (_first_startTime.Ticks == 0)
				{
					_first_startTime = _startTime;
				}
				if (_stopTime.Ticks != 0)
				{
					_interruptMillis += (long)(_startTime - _stopTime).TotalMilliseconds;
				}
			}
		}

		public void Stop()
		{
			StopAsOf(DateTime.UtcNow);
		}

		private void StopAsOf(DateTime when)
		{
			if (_startTime.Ticks != 0)
			{
				_stopTime = when;
				_loggedMillis += (long)(_stopTime - _startTime).TotalMilliseconds;
				_startTime = new DateTime(0, DateTimeKind.Utc);
			}
		}

		public bool IsRunning()
		{
			return _startTime.Ticks != 0;
		}

		public bool IsPaused()
		{
			return _startTime.Ticks == 0;
		}

		public void Reset()
		{
			_first_startTime = new DateTime(0, DateTimeKind.Utc);
			_startTime = new DateTime(0, DateTimeKind.Utc);
			_stopTime = new DateTime(0, DateTimeKind.Utc);
			_loggedMillis = 0;
			_interruptMillis = 0;
		}

		public void SetLoggedMinutes(double minutes)
		{
			if (IsRunning())
			{
				Stop();
				_loggedMillis = (long)(minutes * MINUTES);
				Start();
			}
			else
			{
				_loggedMillis = (long)(minutes * MINUTES);
			}
		}

		public double GetLoggedMinutes()
		{
			Console.WriteLine("stopwatch-getLoggedMinutes: _loggedMillis=" + _loggedMillis + "; isRunning=" + IsRunning());
			long time = (long)(_loggedMillis / MINUTES);
			if (IsRunning())
			{
				time += (long)(DateTime.UtcNow - _startTime).TotalMinutes;
			}
			Console.WriteLine("return: " + time);
			return time;
		}

		public void SetInterruptMinutes(double minutes)
		{
			_interruptMillis = (long)(minutes * MINUTES);
		}

		public double GetInterruptMinutes()
		{
			return _interruptMillis / MINUTES;
		}

		public double GetTrailingLoggedMinutes()
		{
			if (IsRunning())
			{
				Console.WriteLine("getTrailingLoggedMinutes: from " + _startTime.ToString() + " has minutes: " + (DateTime.UtcNow - _startTime).TotalMinutes);
				return (DateTime.UtcNow - _startTime).TotalMinutes;
			}
			else
			{
				Console.WriteLine("getTrailingLoggedMinutes: not running. Return 0 ");
				return 0;
			}
		}

		public double GetTrailingInterruptMinutes()
		{
			if (IsPaused() && _stopTime.Ticks != 0)
			{
				return (DateTime.UtcNow - _stopTime).TotalMinutes;
			}
			else
			{
				return 0;
			}
		}

		public void MaybeCancelRunawayTimer(double maxTrailingLoggedMinutes)
		{
			double trailingLoggedMinutes = GetTrailingLoggedMinutes();
			if (trailingLoggedMinutes < maxTrailingLoggedMinutes)
			{
				return;
			}
			double minutesToCancel = trailingLoggedMinutes - maxTrailingLoggedMinutes;
			long millisToCancel = (long)(minutesToCancel * MINUTES);
			DateTime cancelTime = DateTime.UtcNow.AddMilliseconds(-millisToCancel);
			CancelTimingAsOf(cancelTime);
		}

		public void CancelTimingAsOf(DateTime cancellationTime)
		{
			DateTime now = DateTime.UtcNow;
			if (cancellationTime > now)
			{
				cancellationTime = now;
			}

			if (_startTime.Ticks != 0)
			{
				if (_startTime < cancellationTime)
				{
					StopAsOf(cancellationTime);
					return;
				}
				else
				{
					if (_stopTime.Ticks != 0)
					{
						long interrupToDiscard = (long)(_startTime - _stopTime).TotalMilliseconds;
						_interruptMillis -= interrupToDiscard;
						if (_interruptMillis < 0)
						{
							_interruptMillis = 0;
						}
					}
					_startTime = new DateTime(0, DateTimeKind.Utc);
				}
			}

			if (_stopTime.Ticks != 0 && cancellationTime < _stopTime)
			{
				long overlapMillis = (long)(_stopTime - cancellationTime).TotalMilliseconds;
				if (overlapMillis > _loggedMillis)
				{
					Reset();
				}
				else
				{
					_loggedMillis = _loggedMillis - overlapMillis;
					_stopTime = cancellationTime;
				}
			}
		}
	}
}

