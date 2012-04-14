using System.Collections.Generic;

namespace FubuMVC.Core.Resources.Conneg.New
{
    public interface IReader<T>
    {
        IEnumerable<string> Mimetypes { get; }
        T Read(string mimeType);
    }
}