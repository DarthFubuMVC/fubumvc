using System.IO;
using System.IO.Compression;
using FubuMVC.Core.Http;

namespace FubuMVC.Tests.Http
{
    public class GZipHttpContentEncoding : IHttpContentEncoding
    {
        public ContentEncoding MatchingEncoding
        {
            get { return ContentEncoding.GZip; }
        }

        public Stream Encode(Stream content)
        {
            return new GZipStream(content, CompressionMode.Compress);
        }
    }
}