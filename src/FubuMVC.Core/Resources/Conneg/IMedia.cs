using System.Collections.Generic;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.Resources.Conneg
{
    public interface IMedia
    {
        IEnumerable<string> Mimetypes { get; }
        IConditional Condition { get; }

        object Writer { get; }
    }

    public interface IMedia<in T> : IMedia
    {
        void Write(string mimeType, IFubuRequestContext context, T resource);
        bool MatchesRequest(IFubuRequestContext context);
    }
}