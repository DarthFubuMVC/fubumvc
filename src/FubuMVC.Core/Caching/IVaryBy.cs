using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Caching
{
    public interface IVaryBy
    {
        void Apply(IDictionary<string, string> dictionary);
    }

    public static class VaryByExtensions
    {
        public static IDictionary<string, string> Values(this IVaryBy varyBy)
        {
            var dictionary = new Dictionary<string, string>();

            varyBy.Apply(dictionary);

            return dictionary;
        }
    }
}