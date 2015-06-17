using System;

namespace FubuTransportation.Runtime.Invocation
{
    public class NoHandlerException : Exception
    {
        public NoHandlerException(Type messageType)
            : base("No registered handler for message type " + messageType.FullName)
        {
        }
    }
}