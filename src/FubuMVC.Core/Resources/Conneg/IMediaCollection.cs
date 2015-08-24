using System.Collections.Generic;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public interface IMediaCollection<T> where T : class
    {
        IMediaWriter<T> SelectWriter(CurrentMimeType mimeTypes, IFubuRequestContext logger);
        IEnumerable<IMediaWriter<T>> Writers { get; }
    }
}