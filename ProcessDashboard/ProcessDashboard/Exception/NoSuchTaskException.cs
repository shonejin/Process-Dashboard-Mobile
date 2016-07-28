using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ProcessDashboard
{
    class NoSuchTaskException : System.Exception
    {

        public NoSuchTaskException()
        {

        }

        public NoSuchTaskException(string message)
        : base(message)
        { }

        public NoSuchTaskException(string message, System.Exception innerException)
        : base(message, innerException)
        { }

        protected NoSuchTaskException(SerializationInfo info, StreamingContext context)
        : base(info, context)
        { }
    }
}
