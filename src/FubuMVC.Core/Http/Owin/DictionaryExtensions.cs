using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Http.Owin
{
    public static class DictionaryExtensions
    {
        public static void Set<T>(this IDictionary<string, object> dict, string key, T value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public static void CopyTo(this IDictionary<string, object> source, IDictionary<string, object> destination,
            params string[] keys)
        {
            keys.Where(source.ContainsKey).Each(x => destination.Add(x, source[x]));
        }
    }
}