using System.Collections.Generic;

namespace FubuMVC.Core.Resources.Conneg
{
    public interface IReader<T>
    {
        IEnumerable<string> Mimetypes { get; }
        T Read(string mimeType);
    }
}