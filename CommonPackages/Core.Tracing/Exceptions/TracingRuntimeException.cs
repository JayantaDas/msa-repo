using System;

namespace Core.Tracing
{
    public class TracingRuntimeException : Exception
    {
        public TracingRuntimeException()
            : base()
        {
        }

        public TracingRuntimeException(string message)
            : base(message)
        {
        }
    }
}
