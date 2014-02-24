using System.Collections.Generic;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.Resources.Conneg
{
    public interface IMedia
    {
        IEnumerable<string> Mimetypes { get; }
    }

    public interface IMedia<in T> : IMedia
    {
        IConditional Condition { get; }
        void Write(string mimeType, IFubuRequestContext context, T resource);
        bool MatchesRequest(IFubuRequestContext context);
    }
}