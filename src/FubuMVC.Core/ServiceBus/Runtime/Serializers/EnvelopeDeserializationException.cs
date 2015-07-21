using System;
using System.Runtime.Serialization;

namespace FubuMVC.Core.ServiceBus.Runtime.Serializers
{
    [Serializable]
    public class EnvelopeDeserializationException : Exception
    {
        public EnvelopeDeserializationException(string message) : base(message)
        {
        }

        public EnvelopeDeserializationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public EnvelopeDeserializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}