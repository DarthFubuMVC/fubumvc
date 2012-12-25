using System;
using System.Reflection;


namespace FubuMVC.Autofac
{
    internal static class ReflectionExtensions
    {
        internal static bool TryGetDeclaringProperty(this ParameterInfo parameterInfo, out PropertyInfo propertyInfo)
        {
            var methodInfo = parameterInfo.Member as MethodInfo;
            if (methodInfo != null && methodInfo.IsSpecialName && methodInfo.Name.StartsWith("set_", StringComparison.Ordinal) && methodInfo.DeclaringType != null)
            {
                propertyInfo = methodInfo.DeclaringType.GetProperty(methodInfo.Name.Substring(4));
                return true;
            }

            propertyInfo = null;
            return false;
        }
    }
}