using System;
using System.Runtime.Serialization;

namespace LightningQueues.Exceptions
{
    [Serializable]
    public class UnexpectedReceivedMessageFormatException : Exception
    {
        public UnexpectedReceivedMessageFormatException()
        {
        }

        public UnexpectedReceivedMessageFormatException(string message)
            : base(message)
        {
        }

        public UnexpectedReceivedMessageFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected UnexpectedReceivedMessageFormatException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}