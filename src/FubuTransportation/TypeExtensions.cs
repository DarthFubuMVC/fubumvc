using System;

namespace FubuTransportation
{
    public static class TypeExtensions
    {
        public static bool HasProperty(this Type type, string name)
        {
            return type.GetProperty(name) != null;
        }

    }
}