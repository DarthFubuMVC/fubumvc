using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuMVC.Core.Rest.Media
{
    public interface IValues<T>
    {
        T Subject { get; }
        object ValueFor(Accessor accessor);
    }

    public static class ValuesExtensions
    {
        public static object ValueFor<T>(this IValues<T> values, Expression<Func<T, object>> expression)
        {
            return values.ValueFor(expression.ToAccessor());
        }
    }

    public interface IValueStream<T>
    {
        IEnumerable<IValues<T>> Elements { get; }
    }
}