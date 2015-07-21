using System.Collections.Generic;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public interface IReaderCollection<T> where T : class
    {
        IReader<T> ChooseReader(CurrentMimeType mimeTypes, IFubuRequestContext context);
    }
}