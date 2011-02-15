using System;
using System.Runtime.Serialization;

namespace FubuFastPack.JqGrid
{
    public class SmartGridException : Exception
    {
        public SmartGridException(string message) : base(message)
        {
        }

        public SmartGridException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SmartGridException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}