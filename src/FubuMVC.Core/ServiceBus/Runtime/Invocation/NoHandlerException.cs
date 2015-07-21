using System;
using System.Runtime.Serialization;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    [Serializable]
    public class NoHandlerException : Exception
    {
        public NoHandlerException(Type messageType)
            : base("No registered handler for message type " + messageType.FullName)
        {
        }

        protected NoHandlerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}