using System.Collections.Generic;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Media
{
    public interface IMediaDocument
    {
        IMediaNode Root { get; }

        IEnumerable<string> Mimetypes { get; }
        void Write(IOutputWriter writer);
    }
}