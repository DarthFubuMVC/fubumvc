using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public interface IReader
    {
        IEnumerable<string> Mimetypes { get; }
    }

    public interface IReader<T> : IReader
    {
        Task<T> Read(string mimeType, IFubuRequestContext context);
    }
}