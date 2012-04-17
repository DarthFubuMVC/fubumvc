using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.Urls;

namespace FubuMVC.Media.Projections
{
    public class AccessorProjection<T, TValue> : SingleValueProjection<T>
    {
        private readonly Accessor _accessor;

        public AccessorProjection(Accessor accessor) : base(accessor.Name, context => context.ValueFor(accessor))
        {
            _accessor = accessor;
        }

        public static AccessorProjection<T, TValue> For(Expression<Func<T, TValue>> expression)
        {
            return new AccessorProjection<T, TValue>(ReflectionHelper.GetAccessor(expression));
        }

        public AccessorProjection<T, TValue> Name(string value)
        {
            attributeName = value;
            return this;
        }

        public AccessorProjection<T, TValue> Formatted()
        {
            source = context =>
            {
                return context.FormattedValueOf(_accessor);
            };

            return this;
        }

        public AccessorProjection<T, TValue> FormattedBy(Func<TValue, object> formatting)
        {
            source = context =>
            {
                var raw = context.ValueFor(_accessor);
                if (raw == null)
                {
                    return null;
                }

                return formatting((TValue)raw);
            };

            return this;
        }

        public AccessorProjection<T, TValue> WriteUrlFor(Func<TValue, object> inputBuilder)
        {
            return WriteUrlFor((urls, value) =>
            {
                var inputModel = inputBuilder(value);
                return urls.UrlFor(inputModel);
            });
        }

        public AccessorProjection<T, TValue> WriteUrlFor(Func<IUrlRegistry, TValue, string> urlFinder)
        {
            source = context =>
            {
                var raw = context.ValueFor(_accessor);
                if (raw == null)
                {
                    return string.Empty;
                }

                return urlFinder(context.Urls, (TValue)raw);
            };

            return this;
        }

        public string Name()
        {
            return attributeName;
        }
    }
}