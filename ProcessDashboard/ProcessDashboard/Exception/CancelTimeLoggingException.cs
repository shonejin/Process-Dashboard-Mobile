using System;
using System.Runtime.Serialization;

namespace ProcessDashboard

{
    [Serializable]
    public class CancelTimeLoggingException : System.Exception
    {
        private DateTime _stopTime;
        public CancelTimeLoggingException(string message)
        : base(message)
        { }

        public CancelTimeLoggingException(string message, System.Exception innerException)
        : base(message, innerException)
        { }

        protected CancelTimeLoggingException(SerializationInfo info, StreamingContext context)
        : base(info, context)
        { }

        public CancelTimeLoggingException()
        {

        }
        public CancelTimeLoggingException(DateTime stopTime)
        {
            this._stopTime = stopTime;
        }

        public DateTime GetStopTime()
        {
            return _stopTime;
        }
    }
}