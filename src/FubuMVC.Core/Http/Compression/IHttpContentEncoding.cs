using System.IO;

namespace FubuMVC.Core.Http.Compression
{
    public interface IHttpContentEncoding
    {
        ContentEncoding MatchingEncoding { get; }
        Stream Encode(Stream content);
    }
}