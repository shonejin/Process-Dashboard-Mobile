#region
using System;
using System.Runtime.Serialization;
#endregion
namespace ProcessDashboard
{
    [Serializable]
    public class CannotReachServerException : Exception
    {
        //TODO: Static Analysis
        public CannotReachServerException()
        {
        }

        public CannotReachServerException(string message) : base(message)
        {
        }

        public CannotReachServerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CannotReachServerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}