using System;
using System.Collections.Generic;
using System.Reflection;

namespace FubuMVC.Core.Registration
{
    public static class TypeRegistrationExtensions
    {
        // TODO -- move to FubuCore
        public static IEnumerable<MethodInfo> PublicInstanceMethods(this Type type)
        {
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
        }
    }
}