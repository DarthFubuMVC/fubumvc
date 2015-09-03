using System;
using System.Runtime.Serialization;

namespace LightningQueues.Exceptions
{
    [Serializable]
    public class InvalidAcknowledgementException : Exception
    {
        public InvalidAcknowledgementException()
        {
        }

        public InvalidAcknowledgementException(string message)
            : base(message)
        {
        }

        public InvalidAcknowledgementException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected InvalidAcknowledgementException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}