using System.Collections.Generic;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public interface IMediaWriter<T>
    {
        void Write(string mimeType, IFubuRequestContext context, T resource);
        IEnumerable<string> Mimetypes { get; }
    }
}