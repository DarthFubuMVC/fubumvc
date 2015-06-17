using System;
using System.Runtime.Serialization;

namespace FubuTransportation
{
    [Serializable]
    public class ReceiveFailureException : Exception
    {
        public ReceiveFailureException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ReceiveFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}