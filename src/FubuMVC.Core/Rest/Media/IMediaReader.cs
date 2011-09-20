using System.Collections.Generic;

namespace FubuMVC.Core.Rest.Media
{
    public interface IMediaReader<T>
    {
        T Read(string mimeType);
        IEnumerable<string> Mimetypes { get; }
    }
}