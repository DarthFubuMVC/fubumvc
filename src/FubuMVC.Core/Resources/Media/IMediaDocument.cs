using System.Collections.Generic;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Media
{
    public interface IMediaDocument
    {
        IMediaNode Root { get; }

        IEnumerable<string> Mimetypes { get; }
        void Write(IOutputWriter writer);
    }
}