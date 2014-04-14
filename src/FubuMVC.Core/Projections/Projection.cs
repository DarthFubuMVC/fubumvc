using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using HtmlTags;

namespace FubuMVC.Core.Projections
{
    public class Projection<T> : IProjection<T>, IMediaWriter<T>, DescribesItself
    {
        private DisplayFormatting _formatting;
        private readonly IList<IProjection<T>> _values = new List<IProjection<T>>();

        /// <summary>
        /// Uses raw value formatting
        /// </summary>
        public Projection() : this(DisplayFormatting.RawValues)
        {
        }

        public Projection(DisplayFormatting formatting)
        {
            _formatting = formatting;
        }

        public Projection<T> Filter(Func<Accessor, bool> filter)
        {
            var values = _values.Where(x => x.Accessors().All(filter));
            var projection = new Projection<T>(_formatting);
            projection._values.AddRange(values);

            return projection;
        }

        IEnumerable<Accessor> IProjection<T>.Accessors()
        {
            return _values.SelectMany(x => x.Accessors());
        }

        public DisplayFormatting Formatting
        {
            get { return _formatting; }
            set { _formatting = value; }
        }

        void IProjection<T>.Write(IProjectionContext<T> context, IMediaNode node)
        {
            write(context, node);
        }

        protected void write(IProjectionContext<T> context, IMediaNode node)
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

        public void Include<TProjection>() where TProjection : IProjection<T>, new()
        {
            _values.Add(new DelegatingProjection<T, TProjection>());
        }

        public ChildProjection<T, TChild> Child<TChild>(Expression<Func<T, TChild>> expression) where TChild : class
        {
            var child = new ChildProjection<T, TChild>(expression, _formatting);
            _values.Add(child);

            return child;
        }

        public SingleLineExpression ForAttribute(string attributeName)
        {
            return new SingleLineExpression(attributeName, this);
        }

        public void WriteWith(Action<IProjectionContext<T>, IMediaNode> writer)
        {
            _values.Add(new LambdaProjection<T>(writer));
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

        public EnumerableExpression<TChild> Enumerable<TChild>(Expression<Func<T, IEnumerable<TChild>>> expression)
        {
            var enumerable = EnumerableProjection<T, TChild>.For(expression);
            _values.Add(enumerable);

            return new EnumerableExpression<TChild>(enumerable);
        }

        public class EnumerableExpression<TChild>
        {
            private readonly EnumerableProjection<T, TChild> _enumerable;

            public EnumerableExpression(EnumerableProjection<T, TChild> enumerable)
            {
                _enumerable = enumerable;
            }

            public EnumerableExpression<TChild> NodeName(string nodeName)
            {
                _enumerable.NodeName = nodeName;
                return this;
            }

            public EnumerableExpression<TChild> LeafName(string leafName)
            {
                _enumerable.LeafName = leafName;
                return this;
            }

            public EnumerableExpression<TChild> UseProjection<TProjection>() where TProjection : IProjection<TChild>
            {
                _enumerable.UseProjection<TProjection>();
                return this;
            }

            public EnumerableExpression<TChild> DefineProjection(Action<Projection<TChild>> configuration)
            {
                _enumerable.DefineProjection(configuration);
                return this;
            }
        }


        void IMediaWriter<T>.Write(string mimeType, IFubuRequestContext request, T resource)
        {
            var node = new DictionaryMediaNode();
            var context = new ProjectionContext<T>(request.Services, new SimpleValues<T>(resource));

            write(context, node);

            request.Writer.Write(mimeType, JsonUtil.ToJson(node.Values));
        }

        public virtual IEnumerable<string> Mimetypes
        {
            get
            {
                yield return MimeType.Json.Value;
            }
        }

        public virtual void Describe(Description description)
        {
            description.Title = "Projection {0} for Type {1}".ToFormat(GetType().Name, typeof (T).Name);
        }
    }
}