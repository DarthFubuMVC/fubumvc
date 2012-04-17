using System.Collections.Generic;
using FubuMVC.Core.Runtime;
using FubuMVC.Media.Projections;

namespace FubuMVC.Media
{
    public interface IMediaDocument
    {
        IMediaNode Root { get; }

        IEnumerable<string> Mimetypes { get; }
        void Write(IOutputWriter writer, string mimeType);
    }
}