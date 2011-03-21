using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using FubuCore;

namespace FubuValidation
{
    public static class CoreExtensions
    {
        public static void Fill<KEY, VALUE>(this IDictionary<KEY, VALUE> dictionary, KEY key, VALUE value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                return;
            }

            dictionary.Add(key, value);
        }

        public static int Count(this IEnumerable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var collection = source as ICollection;
            if (collection != null)
            {
                return collection.Count;
            }

            var num = 0;
            var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ++num;
            }

            return num;
        }

        public static bool IsEqualTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            var actualList = actual.ToArray();
            var expectedList = expected.ToArray();

            if (actualList.Length != expectedList.Length)
            {
                return false;
            }

            for (var i = 0; i < actualList.Length; ++i)
            {
                if (!actualList[i].Equals(expectedList[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsEnumerable(this Type type)
        {
            return !type.IsString() && typeof (IEnumerable).IsAssignableFrom(type);
        }

        public static void Each(this NameValueCollection collection, Action<string, string> action)
        {
            collection.AllKeys.Each(key => action(key, collection[key]));
        }
    }
}