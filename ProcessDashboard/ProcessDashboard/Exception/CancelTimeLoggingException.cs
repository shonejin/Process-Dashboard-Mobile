#region
using System;
using System.Runtime.Serialization;
#endregion
namespace ProcessDashboard

{
    [Serializable]
    public class CancelTimeLoggingException : StatusNotOkayException
    {
        //TODO: Static Analysis
        private DateTime _stopTime;

        public CancelTimeLoggingException(string message, string code) : base(message, code)
        {
        }

        public CancelTimeLoggingException(string message)
            : base(message)
        {
        }

        public CancelTimeLoggingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CancelTimeLoggingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public CancelTimeLoggingException()
        {
        }
        public CancelTimeLoggingException(DateTime stopTime)
        {
            _stopTime = stopTime;
        }

        public DateTime GetStopTime()
        {
            return _stopTime;
        }
    }
}