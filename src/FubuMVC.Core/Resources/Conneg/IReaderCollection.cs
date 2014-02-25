using System.Collections.Generic;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Resources.Conneg
{
    public interface IReaderCollection<T> where T : class
    {
        IReader<T> ChooseReader(CurrentMimeType mimeTypes, IFubuRequestContext context);
    }
}