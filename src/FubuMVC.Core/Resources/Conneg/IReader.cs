using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Resources.Conneg
{
    public interface IReader
    {
        IEnumerable<string> Mimetypes { get; }
    }

    public interface IReader<T> : IReader
    {
        T Read(string mimeType, IFubuRequestContext context);
    }
}