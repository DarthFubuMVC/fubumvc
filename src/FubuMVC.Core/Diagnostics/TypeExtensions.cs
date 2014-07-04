using System;
using System.Collections.Generic;

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
    }
}