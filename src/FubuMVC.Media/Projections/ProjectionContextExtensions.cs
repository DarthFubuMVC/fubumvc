using System;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuMVC.Media.Projections
{
    public static class ProjectionContextExtensions
    {
        public static string FormattedValueOf<T>(this IProjectionContext<T> context, Accessor accessor)
        {
            return context.Formatter.GetDisplayForValue(accessor, context.ValueFor(accessor));
        }

        public static string FormattedValueOf<T>(this IProjectionContext<T> context, Expression<Func<T, object>> expression)
        {
            return context.FormattedValueOf(expression.ToAccessor());
        }
    }
}