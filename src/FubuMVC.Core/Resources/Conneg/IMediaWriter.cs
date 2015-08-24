using System.Collections.Generic;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public interface IMediaWriter
    {
        IEnumerable<string> Mimetypes { get; }
    }

    public interface IMediaWriter<T> : IMediaWriter
    {
        void Write(string mimeType, IFubuRequestContext context, T resource);
        
    }
}