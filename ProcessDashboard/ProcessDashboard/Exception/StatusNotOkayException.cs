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
        private string _message;

        public string GetMessage()
        {
            return "Code :" + _code + " Message :" + _message;
        }

        public StatusNotOkayException()
        {
        }

        public StatusNotOkayException(string message, string code)
        {
            this._message = message;
            _code = code;
        }

        public StatusNotOkayException(string message)
        {
            this._message = message;
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
        private StreamingContext Context { get; }
        private SerializationInfo Info { get; }
        private new Exception InnerException { get; }

        public override string ToString()
        {
            return GetMessage();
        }
    }
}