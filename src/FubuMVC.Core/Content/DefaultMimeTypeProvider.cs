using FubuCore.Util;

namespace FubuMVC.Core.Content
{
    public class DefaultMimeTypeProvider : IMimeTypeProvider
    {
        private static Cache<string, string> _mimeTypes = new Cache<string, string>();

        static DefaultMimeTypeProvider()
        {
            _mimeTypes[".gif"] = "image/gif";
            _mimeTypes[".png"] = "image/png";
            _mimeTypes[".jpg"] = "image/jpeg";
            _mimeTypes[".jpeg"] = "image/jpeg";
            _mimeTypes[".bm"] = "image/bmp";
            _mimeTypes[".bmp"] = "image/bmp";
            _mimeTypes[".css"] = "text/css";
            _mimeTypes[".js"] = "application/x-javascript";
        }

        public string For(string extension)
        {
            return _mimeTypes[extension];
        }

        public void Register(string extension, string mimeType)
        {
            _mimeTypes[extension] = mimeType;
        }
    }
}