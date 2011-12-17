using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Resources.Media.Projections
{
    public interface IValueProjection<T>
    {
        void WriteValue(IProjectionContext<T> target, IMediaNode node);
    }

    public class SingleValueProjection<T> : IValueProjection<T>
    {
        public SingleValueProjection(string attributeName, Func<IProjectionContext<T>, object> source)
        {
            this.attributeName = attributeName;
            this.source = source;
        }

        protected string attributeName { get; set; }

        protected Func<IProjectionContext<T>, object> source { get; set; }

        public void WriteValue(IProjectionContext<T> target, IMediaNode node)
        {
            node.SetAttribute(attributeName, source(target));
        }
    }

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

        public AccessorProjection<T, TValue> FormattedBy(Func<TValue, string> formatting)
        {
            source = context =>
            {
                var raw = context.ValueFor(_accessor);
                if (raw == null)
                {
                    return string.Empty;
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

    public class Projection<T> : IValueProjection<T>
    {
        private readonly IList<IValueProjection<T>> _values = new List<IValueProjection<T>>();


        void IValueProjection<T>.WriteValue(IProjectionContext<T> target, IMediaNode node)
        {
            _values.Each(x => x.WriteValue(target, node));
        }

        public AccessorProjection<T, TValue> Value<TValue>(Expression<Func<T, TValue>> expression)
        {
            var value = new AccessorProjection<T, TValue>(ReflectionHelper.GetAccessor(expression));
            _values.Add(value);

            return value;
        }

        public void Include<TProjection>() where TProjection : IValueProjection<T>
        {
            _values.Add(new DelegatingProjection<T, TProjection>());
        }

        public SingleLineExpression ForAttribute(string attributeName)
        {
            return new SingleLineExpression(attributeName, this);
        }

        public class SingleLineExpression
        {
            private readonly string _attributeName;
            private readonly Projection<T> _parent;

            public SingleLineExpression(string attributeName, Projection<T> parent)
            {
                _attributeName = attributeName;
                _parent = parent;
            }

            public void Use(Func<IProjectionContext<T>, object> source)
            {
                var projection = new SingleValueProjection<T>(_attributeName, source);
                _parent._values.Add(projection);
            }

            public PropertyExpression<TValue> ValueFrom<TValue>(Expression<Func<T, TValue>> expression)
            {
                return new PropertyExpression<TValue>(this, ReflectionHelper.GetAccessor(expression));
            }

            public class PropertyExpression<TValue>
            {
                private readonly SingleLineExpression _parent;
                private readonly Accessor _accessor;

                public PropertyExpression(SingleLineExpression parent, Accessor accessor)
                {
                    _parent = parent;
                    _accessor = accessor;
                }

                public void Use(Func<TValue, string> source)
                {
                    _parent.Use(context =>
                    {
                        var raw = context.ValueFor(_accessor);
                        if (raw == null)
                        {
                            return string.Empty;
                        }

                        return source((TValue) raw);
                    });
                }
            }
        }


    }
}