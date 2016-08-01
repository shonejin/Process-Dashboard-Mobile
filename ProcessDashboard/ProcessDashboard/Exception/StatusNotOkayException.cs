#region
using System;
using System.Runtime.Serialization;
#endregion
namespace ProcessDashboard
{
    [Serializable]
    public class StatusNotOkayException : Exception
    {
        private string _code;
        private string message;

        public StatusNotOkayException()
        {
        }

        public StatusNotOkayException(string message, string code)
        {
            this.message = message;
            _code = code;
        }

        public StatusNotOkayException(string message)
        {
            this.message = message;
        }

        public StatusNotOkayException(SerializationInfo info, StreamingContext context)
        {
            Info = info;
            Context = context;
        }

        public StatusNotOkayException(string message, Exception innerException) : this(message)
        {
            InnerException = innerException;
        }
        //TODO: Check if these things can be removed
        //TODO: Static Analysis
        public StreamingContext Context { get; }
        public SerializationInfo Info { get; }
        public new Exception InnerException { get; }

        public override string ToString()
        {
            return "Messsage : " + message + "Code :" + _code;
        }
    }
}