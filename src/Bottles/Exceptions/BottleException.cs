using System;
using System.Runtime.Serialization;

namespace Bottles.Exceptions
{
    [Serializable]
    public class BottleException : Exception
    {
        public BottleException()
        {
        }

        public BottleException(string message) : base(message)
        {
        }

        public BottleException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BottleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}