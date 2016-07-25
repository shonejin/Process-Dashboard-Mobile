using System;
using System.Runtime.Serialization;

namespace ProcessDashboard
{
    [Serializable]
	public class CannotReachServerException : System.Exception
    {
		public CannotReachServerException()
		{
		    
		}

        public CannotReachServerException(string message) 
        : base(message)
    { }

        public CannotReachServerException(string message, Exception innerException)
        : base (message, innerException)
    { }

        protected CannotReachServerException(SerializationInfo info, StreamingContext context)
        : base (info, context)
    { }
    }
}

