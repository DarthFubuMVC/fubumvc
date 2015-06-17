using System;
using System.Runtime.Serialization;
using FubuCore;

namespace FubuTransportation.Runtime
{
    [Serializable]
    public class UnknownChannelException : Exception
    {
        public UnknownChannelException(Uri uri) : base("Unknown destination '{0}'".ToFormat(uri))
        {
        }

        protected UnknownChannelException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}