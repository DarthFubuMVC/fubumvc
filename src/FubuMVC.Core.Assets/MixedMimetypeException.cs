using System;
using System.Runtime.Serialization;

namespace FubuMVC.Core.Assets
{
    [Serializable]
    public class MixedMimetypeException : Exception
    {
        public MixedMimetypeException(string message) : base(message)
        {
        }

        protected MixedMimetypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}