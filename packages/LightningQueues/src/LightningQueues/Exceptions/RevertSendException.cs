using System;
using System.Runtime.Serialization;

namespace LightningQueues.Exceptions
{
    [Serializable]
    public class RevertSendException : Exception
    {
        public RevertSendException()
        {
        }

        public RevertSendException(string message)
            : base(message)
        {
        }

        public RevertSendException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected RevertSendException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}