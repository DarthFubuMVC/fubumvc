using System.Collections.Generic;

namespace FubuMVC.Core.Rest.Media
{
    public interface IMediaReader<T>
    {
        IEnumerable<string> Mimetypes { get; }
        T Read(string mimeType);
    }
}