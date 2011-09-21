using System.Collections.Generic;

namespace FubuMVC.Core.Rest.Media.Formatters
{
    public interface IFormatter
    {
        IEnumerable<string> MatchingMimetypes { get; }
        void Write<T>(T target);
        T Read<T>();
    }
}