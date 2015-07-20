using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace FubuMVC.Core.Json
{
    public static class JTokenKeyValuePairExtensions
    {
        public static void IfIs<T>(this KeyValuePair<string, JToken> pair, Action<T> action) where T : class
        {
            var target = pair.Value as T;

            if (target != null)
            {
                action(target);
            }
        }
    }
}