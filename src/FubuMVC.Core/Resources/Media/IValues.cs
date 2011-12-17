using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Resources.Media
{
    public interface IValues<T>
    {
        T Subject { get; }
        object ValueFor(Accessor accessor);
    }

    public interface IProjectionContext<T> : IValues<T>
    {
        TService Service<TService>();
        IUrlRegistry Urls { get; }
        IDisplayFormatter Formatter { get; }
    }

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

    // TODO -- might want something besides IDisplayFormatter
    [MarkedForTermination("Pulling inside something else instead")]
    public class FormattedValues<T> : IValues<T>
    {
        private readonly IDisplayFormatter _formatter;
        private readonly IValues<T> _inner;

        public FormattedValues(IDisplayFormatter formatter, IValues<T> inner)
        {
            _formatter = formatter;
            _inner = inner;
        }

        public object ValueFor(Accessor accessor)
        {
            var innerValue = _inner.ValueFor(accessor);
            return _formatter.GetDisplay(accessor, innerValue);
        }

        public T Subject
        {
            get { return _inner.Subject; }
        }
    }

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

    public interface IValueStream<T>
    {
        IEnumerable<IValues<T>> Elements { get; }
    }
}