using System;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuMVC.Core.Projections
{
    public static class ValuesExtensions
    {
        public static object ValueFor<T>(this IValues<T> values, Expression<Func<T, object>> expression)
        {
            return values.ValueFor(expression.ToAccessor());
        }

        public static IValues<T> ToValues<T>(this T subject)
        {
            return new SimpleValues<T>(subject);
        }
    }
}