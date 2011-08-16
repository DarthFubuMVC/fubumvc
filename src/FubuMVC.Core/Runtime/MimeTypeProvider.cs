using System;
using System.Runtime.Serialization;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;
using System.Linq;

namespace FubuMVC.Core.Runtime
{


    // TODO -- Make it easy to alter this
    // in FubuRegistry
    public class MimeTypeProvider : IMimeTypeProvider
    {
        public MimeType For(string extension)
        {
            return MimeType.All().FirstOrDefault(x => x.HasExtension(extension));
        }

        public MimeType For(string extension, AssetFolder folder)
        {
            var mimeType = For(extension);
            if (mimeType != null) return mimeType;

            switch (folder)
            {
                case AssetFolder.scripts:
                    return MimeType.Javascript;

                case AssetFolder.styles:
                    return MimeType.Css;

                default:
                    throw new UnknownExtensionException(extension);
            }
        }

    }

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