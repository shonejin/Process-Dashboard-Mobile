#region
using System;
using System.Runtime.Serialization;
#endregion
namespace ProcessDashboard
{
    [Serializable]
    internal class NoSuchTaskException : StatusNotOkayException
    {
        //TODO: Static Analysis
        public NoSuchTaskException()
        {
        }

        public NoSuchTaskException(string message, string code) : base(message, code)
        {
        }
        public NoSuchTaskException(string message)
            : base(message)
        {
        }

        public NoSuchTaskException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NoSuchTaskException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}