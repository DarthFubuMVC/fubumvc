using System;
using System.Runtime.Serialization;

namespace FubuMVC.Core.Conneg
{
    [Serializable]
    public class MediaProcessingException : Exception
    {
        public MediaProcessingException(string message) : base(message)
        {
        }

        protected MediaProcessingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}