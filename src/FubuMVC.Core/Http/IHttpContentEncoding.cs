using System.IO;

namespace FubuMVC.Core.Http
{
    public interface IHttpContentEncoding
    {
        ContentEncoding MatchingEncoding { get; }
        Stream Encode(Stream content);
    }
}