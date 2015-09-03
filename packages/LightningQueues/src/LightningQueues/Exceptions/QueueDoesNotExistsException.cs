using System;
using System.Runtime.Serialization;

namespace LightningQueues.Exceptions
{
    [Serializable]
    public class QueueDoesNotExistsException : Exception
    {
        public QueueDoesNotExistsException()
        {
        }

        public QueueDoesNotExistsException(string message) : base(message)
        {
        }

        public QueueDoesNotExistsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected QueueDoesNotExistsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}