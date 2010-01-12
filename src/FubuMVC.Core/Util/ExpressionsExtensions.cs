using System;
using System.Linq.Expressions;

namespace FubuMVC.Core.Util
{
    public static class ExpressionsExtensions
    {
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