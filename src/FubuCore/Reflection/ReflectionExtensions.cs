using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core.Util;

namespace FubuCore.Reflection
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

            return (U) (result ?? default(U));
        }

        public static T GetAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            object[] atts = provider.GetCustomAttributes(typeof (T), true);
            return atts.Length > 0 ? atts[0] as T : null;
        }

        public static bool HasAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            object[] atts = provider.GetCustomAttributes(typeof (T), true);
            return atts.Length > 0;
        }

        public static void ForAttribute<T>(this ICustomAttributeProvider provider, Action<T> action) where T : Attribute
        {
            foreach (T attribute in provider.GetCustomAttributes(typeof (T), true))
            {
                action(attribute);
            }
        }

        public static void ForAttribute<T>(this Accessor accessor, Action<T> action) where T : Attribute
        {
            foreach (T attribute in accessor.InnerProperty.GetCustomAttributes(typeof (T), true))
            {
                action(attribute);
            }
        }

        public static T GetAttribute<T>(this Accessor provider) where T : Attribute
        {
            return provider.InnerProperty.GetAttribute<T>();
        }

        public static bool HasAttribute<T>(this Accessor provider) where T : Attribute
        {
            return provider.InnerProperty.HasAttribute<T>();
        }

        public static Accessor ToAccessor<T>(this Expression<Func<T, object>> expression)
        {
            return ReflectionHelper.GetAccessor(expression);
        }

        public static string GetName<T>(this Expression<Func<T, object>> expression)
        {
            return ReflectionHelper.GetAccessor(expression).Name;
        }
    }
}