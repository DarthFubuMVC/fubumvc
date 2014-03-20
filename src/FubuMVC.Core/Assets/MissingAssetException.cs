using System;
using System.Runtime.Serialization;

namespace FubuMVC.Core.Assets
{
    [Serializable]
    public class MissingAssetException : Exception
    {
        public MissingAssetException(string message) : base(message)
        {
        }

        protected MissingAssetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}