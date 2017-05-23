using System;

namespace LightningQueues.Storage
{
    public class QueueDoesNotExistException : Exception
    {
        public QueueDoesNotExistException()
        {
        }

        public QueueDoesNotExistException(string message) : base(message)
        {
        }

        public QueueDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}