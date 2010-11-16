using System.Collections.Generic;
using System.Linq;

namespace FubuValidation
{
    public static class CoreExtensions
    {
        public static void Fill<KEY, VALUE>(this IDictionary<KEY, VALUE> dictionary, KEY key, VALUE value)
        {
            if(dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                return;
            }

            dictionary.Add(key, value);
        }

        public static bool IsEqualTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            var actualList = actual.ToArray();
            var expectedList = expected.ToArray();

            if(actualList.Length != expectedList.Length)
            {
                return false;
            }

            for (int i = 0; i < actualList.Length; ++i)
            {
                if(!actualList[i].Equals(expectedList[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}