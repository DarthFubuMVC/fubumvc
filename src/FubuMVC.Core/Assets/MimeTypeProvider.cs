using System;
using System.Runtime.Serialization;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets
{
    // TODO -- Make it easy to alter this
    // in FubuRegistry
    public class MimeTypeProvider : IMimeTypeProvider
    {
        public const string JAVASCRIPT = "application/x-javascript";
        public const string CSS = "text/css";


        private readonly Cache<string, string> _mimeTypes = new Cache<string, string>(extension =>
        {
            throw new UnknownExtensionException(extension);
        });

        public MimeTypeProvider()
        {
            _mimeTypes[".gif"] = "image/gif";
            _mimeTypes[".png"] = "image/png";
            _mimeTypes[".jpg"] = "image/jpeg";
            _mimeTypes[".jpeg"] = "image/jpeg";
            _mimeTypes[".bm"] = "image/bmp";
            _mimeTypes[".bmp"] = "image/bmp";
            _mimeTypes[".css"] = CSS;
            _mimeTypes[".js"] = JAVASCRIPT;
        }

        public string For(string extension)
        {
            return _mimeTypes[extension];
        }

        public string For(string extension, AssetFolder folder)
        {
            if (_mimeTypes.Has(extension))
            {
                return _mimeTypes[extension];
            }

            switch (folder)
            {
                case AssetFolder.scripts:
                    return JAVASCRIPT;

                case AssetFolder.styles:
                    return CSS;

                default:
                    throw new UnknownExtensionException(extension);
            }
        }

        public void Register(string extension, string mimeType)
        {
            _mimeTypes[extension] = mimeType;
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