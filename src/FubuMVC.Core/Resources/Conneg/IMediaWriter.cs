using System.Collections.Generic;

namespace FubuMVC.Core.Resources.Conneg
{
    public interface IMediaWriter<T>
    {
        void Write(string mimeType, IFubuRequestContext context, T resource);
        IEnumerable<string> Mimetypes { get; }
    }
}