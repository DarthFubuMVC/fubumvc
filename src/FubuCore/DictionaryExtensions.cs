using System.Collections.Generic;

namespace FubuCore
{
    public static class DictionaryExtensions
    {
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.Get(key, default(TValue));
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary.ContainsKey(key)) return dictionary[key];
            return defaultValue;
        }
    }
}