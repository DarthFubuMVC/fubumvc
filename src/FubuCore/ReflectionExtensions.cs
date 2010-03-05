using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core.Util;

namespace FubuCore
{
    public static class ReflectionExtensions
    {
        public static U ValueOrDefault<T, U>(this T root, Expression<Func<T, U>> expression)
            where T : class
        {
            if (root == null)
            {
                return default(U);
            }

            Accessor accessor = ReflectionHelper.GetAccessor(expression);

            object result = accessor.GetValue(root);

            return (U)(result ?? default(U));
        }

        public static ATTRIBUTE GetCustomAttribute<ATTRIBUTE>(this MemberInfo member)
            where ATTRIBUTE : Attribute
        {
            return member.GetCustomAttributes(typeof(ATTRIBUTE), false).FirstOrDefault() as ATTRIBUTE;
        }

        public static bool HasCustomAttribute<ATTRIBUTE>(this MemberInfo member)
            where ATTRIBUTE : Attribute
        {
            return member.GetCustomAttributes(typeof(ATTRIBUTE), false).Any();
        }
    }
}