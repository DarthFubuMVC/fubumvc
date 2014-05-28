using System.IO;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.Assets
{
    public class Asset
    {
        public readonly IFubuFile File;
        public readonly string Url;
        public readonly MimeType MimeType;
        public readonly string Filename;

        public string CdnUrl;
        public string FallbackTest ;

        // For testing only!
        public Asset(string url)
        {
            Url = url;
            Filename = Path.GetFileName(url);
        }

        public Asset(IFubuFile file)
        {
            File = file;

            Filename = Path.GetFileName(file.Path);
            MimeType = MimeType.MimeTypeByFileName(Filename);

            Url = file.RelativePath.Replace("\\", "/").TrimStart('/');
        }

        public override string ToString()
        {
            return string.Format("Filename: {0}, Url: {1}, MimeType: {2}", Filename, Url, MimeType.Value);
        }
    }
}