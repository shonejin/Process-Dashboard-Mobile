using System;
using System.Runtime.Serialization;

namespace ProcessDashboard

{
    [Serializable]
    public class CancelTimeLoggingException : System.Exception
    {
        private DateTime stopTime;
        public CancelTimeLoggingException(string message)
        : base(message)
        { }

        public CancelTimeLoggingException(string message, Exception innerException)
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
            this.stopTime = stopTime;
        }

        public DateTime getStopTime()
        {
            return stopTime;
        }
    }
}