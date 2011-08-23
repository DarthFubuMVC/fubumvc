using System;
using System.Runtime.Serialization;

namespace FubuMVC.Core.Runtime
{
    [Serializable]
    public class UnknownExtensionException : Exception
    {
        public UnknownExtensionException(string extension) : base("No mimetype registered or known for extension " + extension)
        {
        }

        protected UnknownExtensionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}