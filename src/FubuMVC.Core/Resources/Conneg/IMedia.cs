using System.Collections.Generic;

namespace FubuMVC.Core.Resources.Conneg
{
    public interface IMedia<in T>
    {
        IEnumerable<string> Mimetypes { get; }
        void Write(string mimeType, T resource);
        bool MatchesRequest();
    }
}