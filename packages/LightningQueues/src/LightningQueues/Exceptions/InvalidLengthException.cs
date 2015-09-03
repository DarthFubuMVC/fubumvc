using System;
using System.Runtime.Serialization;

namespace LightningQueues.Exceptions
{
    [Serializable]
    public class InvalidLengthException : Exception
    {
        public InvalidLengthException()
        {
        }

        public InvalidLengthException(string message)
            : base(message)
        {
        }

        public InvalidLengthException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected InvalidLengthException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}