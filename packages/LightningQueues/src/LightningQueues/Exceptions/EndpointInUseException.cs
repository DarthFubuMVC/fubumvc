using System;
using System.Net;
using System.Runtime.Serialization;
using FubuCore;

namespace LightningQueues.Exceptions
{
    [Serializable]
    public class EndpointInUseException : Exception
    {
        public EndpointInUseException(IPEndPoint endpoint, Exception inner) : this("The endpoint {0} is already in use".ToFormat(endpoint), inner)
        {
        }

        public EndpointInUseException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected EndpointInUseException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}