using System.Collections.Generic;

namespace FubuCore
{
    public static class DictionaryExtensions
    {
        public static VALUE Get<KEY, VALUE>(this IDictionary<KEY, VALUE> dictionary, KEY key)
        {
            return dictionary.Get(key, default(VALUE));
        }

        public static VALUE Get<KEY, VALUE>(this IDictionary<KEY, VALUE> dictionary, KEY key, VALUE defaultValue)
        {
            if (dictionary.ContainsKey(key)) return dictionary[key];
            return defaultValue;
        }
    }
}