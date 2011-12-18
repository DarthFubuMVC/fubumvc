using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuMVC.Core.Resources.Media.Projections
{
    public class Projection<T> : IProjection<T>
    {
        private DisplayFormatting _formatting;
        private readonly IList<IProjection<T>> _values = new List<IProjection<T>>();

        public Projection(DisplayFormatting formatting)
        {
            _formatting = formatting;
        }

        public DisplayFormatting Formatting
        {
            get { return _formatting; }
            set { _formatting = value; }
        }

        void IProjection<T>.Write(IProjectionContext<T> context, IMediaNode node)
        {
            _values.Each(x => x.Write(context, node));
        }

        public AccessorProjection<T, TValue> Value<TValue>(Expression<Func<T, TValue>> expression)
        {
            var value = new AccessorProjection<T, TValue>(ReflectionHelper.GetAccessor(expression));
            if (_formatting == DisplayFormatting.UseDisplayFormatting)
            {
                value.Formatted();
            }
            
            
            _values.Add(value);

            return value;
        }

        public void Include<TProjection>() where TProjection : IProjection<T>
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