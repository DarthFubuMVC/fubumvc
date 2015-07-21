using System.Collections.Generic;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public interface IMediaCollection<T> where T : class
    {
        IMedia<T> SelectMedia(CurrentMimeType mimeTypes, IFubuRequestContext logger);
        IEnumerable<IMedia<T>> Media { get; }
    }
}