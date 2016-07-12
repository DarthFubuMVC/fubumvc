using System.Collections.Generic;
using System.Threading.Tasks;

namespace FubuMVC.Core.Runtime.Formatters
{
    public interface IFormatter
    {
        IEnumerable<string> MatchingMimetypes { get; }
        Task Write<T>(IFubuRequestContext context, T target, string mimeType);
        Task<T> Read<T>(IFubuRequestContext context);
    }
}