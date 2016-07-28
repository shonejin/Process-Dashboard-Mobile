using System;
namespace ProcessDashboard
{
	public class Stopwatch
	{
		private DateTime _firstStartTime = new DateTime(0);
		private DateTime _startTime = new DateTime(0);
		private DateTime _stopTime = new DateTime(0);
		private long _loggedMillis = 0;
		private long _interruptMillis = 0;
		private const double Minutes = 60000.0;

		public DateTime GetFirstStartTime()
		{
			return _firstStartTime;
		}

		public void Start()
		{
			if (_startTime.Ticks == 0)
			{
				_startTime = DateTime.Now;
				if (_firstStartTime.Ticks == 0)
				{
					_firstStartTime = _startTime;
				}
				if (_stopTime.Ticks != 0)
				{
					_interruptMillis += (long)(_startTime - _stopTime).TotalMilliseconds;
				}
			}
		}

		public void Stop()
		{
			StopAsOf(DateTime.Now);
		}

		private void StopAsOf(DateTime when)
		{
			if (_startTime.Ticks != 0)
			{
				_stopTime = when;
				_loggedMillis += (long)(_stopTime - _startTime).TotalMilliseconds;
				_startTime = new DateTime(0);
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
			_firstStartTime = new DateTime(0);
			_startTime = new DateTime(0);
			_stopTime = new DateTime(0);
			_loggedMillis = 0;
			_interruptMillis = 0;
		}

		public void SetLoggedMinutes(double minutes)
		{
			if (IsRunning())
			{
				Stop();
				_loggedMillis = (long)(minutes * Minutes);
				Start();
			}
			else
			{
				_loggedMillis = (long)(minutes * Minutes);
			}
		}

		public double GetLoggedMinutes()
		{
			long time = (long)(_loggedMillis / Minutes);
			if (IsRunning())
			{
				time += (long)(DateTime.Now - _startTime).TotalMinutes;
			}
			return time;
		}

		public void SetInterruptMinutes(double minutes)
		{
			_interruptMillis = (long)(minutes * Minutes);
		}

		public double GetInterruptMinutes()
		{
			return _interruptMillis / Minutes;
		}

		public double GetTrailingLoggedMinutes()
		{
			if (IsRunning())
			{
				return (DateTime.Now - _startTime).TotalMinutes;
			}
			else
			{
				return 0;
			}
		}

		public double GetTrailingInterruptMinutes()
		{
			if (IsPaused() && _stopTime.Ticks != 0)
			{
				return (DateTime.Now - _stopTime).TotalMinutes;
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
			long millisToCancel = (long)(minutesToCancel * Minutes);
			DateTime cancelTime = DateTime.Now.AddMilliseconds(-millisToCancel);
			CancelTimingAsOf(cancelTime);
		}

		public void CancelTimingAsOf(DateTime cancellationTime)
		{
			DateTime now = DateTime.Now;
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
					_startTime = new DateTime(0);
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

