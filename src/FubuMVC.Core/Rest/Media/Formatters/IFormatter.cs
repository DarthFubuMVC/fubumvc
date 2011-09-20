using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Rest.Media.Formatters
{
    public interface IFormatter
    {
        void Write<T>(T target);
        T Read<T>();
        IEnumerable<string> MatchingMimetypes { get; }
    }
}