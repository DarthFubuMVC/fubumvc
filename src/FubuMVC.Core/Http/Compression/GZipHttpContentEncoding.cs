using System.IO;
using System.IO.Compression;

namespace FubuMVC.Core.Http.Compression
{
    public class GZipHttpContentEncoding : IHttpContentEncoding
    {
        public ContentEncoding MatchingEncoding
        {
            get { return ContentEncoding.GZip; }
        }

        public Stream Encode(Stream content)
        {
            return new GZipStream(content, CompressionMode.Compress, true);
        }
    }
}