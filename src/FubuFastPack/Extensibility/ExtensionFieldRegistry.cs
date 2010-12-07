using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Util;

namespace FubuFastPack.Extensibility
{
    public static class ExtensionFieldRegistry
    {
        private static readonly Cache<Type, IEnumerable<PropertyInfo>> _viewProps
            = new Cache<Type, IEnumerable<PropertyInfo>>(findProperties);

        private static IEnumerable<PropertyInfo> findProperties(Type type)
        {
            return type.GetProperties().Where(x => x.DeclaringType == type);
        }

        public static IEnumerable<PropertyInfo> Properties<T>()
        {
            return _viewProps[typeof(T)];
        }
    }
}