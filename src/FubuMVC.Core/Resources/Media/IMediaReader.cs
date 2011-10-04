using System.Collections.Generic;

namespace FubuMVC.Core.Resources.Media
{
    public interface IMediaReader<T>
    {
        IEnumerable<string> Mimetypes { get; }
        T Read(string mimeType);
    }
}