using System;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public class NoHandlerException : Exception
    {
        public NoHandlerException(Type messageType)
            : base("No registered handler for message type " + messageType.FullName)
        {
        }
    }
}