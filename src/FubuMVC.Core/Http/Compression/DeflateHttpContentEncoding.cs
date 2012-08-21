using System.IO;
using System.IO.Compression;

namespace FubuMVC.Core.Http.Compression
{
    public class DeflateHttpContentEncoding : IHttpContentEncoding
    {
        public ContentEncoding MatchingEncoding
        {
            get { return ContentEncoding.Deflate; }
        }

        public Stream Encode(Stream content)
        {
            return new DeflateStream(content, CompressionMode.Compress);
        }
    }
}