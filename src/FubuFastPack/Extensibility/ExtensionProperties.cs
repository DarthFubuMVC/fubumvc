using System;
using System.Collections.Generic;
using FubuCore.Util;

namespace FubuFastPack.Extensibility
{
    public static class ExtensionProperties
    {
        private static readonly Cache<Type, Type> _types = new Cache<Type, Type>();

        public static void Register(Type entityType, Type extensionType)
        {
            _types[entityType] = extensionType;
        }

        public static void ClearAll()
        {
            _types.ClearAll();
        }

        public static IEnumerable<Type> ExtensionTypes { get { return _types; } }

        public static bool HasExtensionFor(Type entityType)
        {
            return _types.Has(entityType);
        }

        public static Type ExtensionFor(Type type)
        {
            return _types[type];
        }
    }
}