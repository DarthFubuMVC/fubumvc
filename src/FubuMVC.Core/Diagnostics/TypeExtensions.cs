using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.Core.Diagnostics
{
    public static class TypeExtensions
    {
        public static Dictionary<string, object> ToDictionary(this Type type)
        {
            if (type == null) return null;

            return new Dictionary<string, object>
            {
                {"name", type.Name},
                {"fullname", type.FullName},
                {"qualified-name", type.AssemblyQualifiedName},
                {"namespace", type.Namespace},
                {"assembly", type.Assembly.GetName().Name}
            };
        }

        public static Dictionary<string, object> AddChild(this IDictionary<string, object> parent, string key)
        {
            var dict = new Dictionary<string, object>();
            parent.Add(key, dict);

            return dict;
        }

        public static void AddHeaders(this IDictionary<string, object> parent, string name, IEnumerable<Header> headers)
        {
            var values = headers.GroupBy(x => x.Name).Select(g => {
                return new {key = g.Key, values = g.Select(x => x.Value).Join(", ")};
            }).ToArray();

            parent.Add(name, values);
        }

        public static void AddNameValues(this IDictionary<string, object> parent, string name,
            NameValueCollection values)
        {

            var headers = values.AllKeys.Select(key => {
                return new {key = key, value = values[key]};
            }).ToArray();

            parent.Add(name, headers);
        }
    }
}