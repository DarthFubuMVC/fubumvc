using System;
using System.Runtime.Serialization;

namespace LightningQueues.Exceptions
{
    [Serializable]
    public class FailedToConnectException : Exception
    {
        public FailedToConnectException()
        {
        }

        public FailedToConnectException(string message)
            : base(message)
        {
        }

        public FailedToConnectException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected FailedToConnectException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}