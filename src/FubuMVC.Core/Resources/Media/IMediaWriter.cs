using System.Collections.Generic;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Media
{
    public interface IMediaWriter<T>
    {
        IEnumerable<string> Mimetypes { get; }
        void Write(IValues<T> source, IOutputWriter writer);
        void Write(T source, IOutputWriter writer);
    }
}