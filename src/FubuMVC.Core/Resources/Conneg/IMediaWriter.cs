using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public interface IMediaWriter
    {
        IEnumerable<string> Mimetypes { get; }
    }

    public interface IMediaWriter<T> : IMediaWriter
    {
        Task Write(string mimeType, IFubuRequestContext context, T resource);
        
    }
}