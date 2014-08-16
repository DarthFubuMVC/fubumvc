using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using HtmlTags;

namespace FubuMVC.Core.Projections
{

    public interface IAccessorNaming
    {
        string Name(Accessor accessor);
    }

    public class NormalNaming : IAccessorNaming
    {
        public string Name(Accessor accessor)
        {
            return accessor.Name;
        }
    }

    public class CamelCaseNaming : IAccessorNaming
    {
        public string Name(Accessor accessor)
        {
            char[] text = accessor.Name.ToCharArray();
            text[0] = CultureInfo.CurrentCulture.TextInfo.ToLower(text[0]);

            return new string(text);
        }
    }

    public class Projection<T> : IProjection<T>, IMediaWriter<T>, DescribesItself
    {
        private DisplayFormatting _formatting;
        private readonly IList<IProjection<T>> _values = new List<IProjection<T>>();
        private IAccessorNaming _naming = new NormalNaming();

        public IAccessorNaming Naming
        {
            get
            {
                return _naming;
            }
            set
            {
                _naming = value ?? new NormalNaming();
                _values.OfType<IAccessorProjection>().Each(x => x.ApplyNaming(_naming));
            }
        }

        /// <summary>
        /// Uses raw value formatting
        /// </summary>
        public Projection()
            : this(DisplayFormatting.RawValues)
        {
        }

        public Projection(DisplayFormatting formatting)
        {
            _formatting = formatting;
        }

        /// <summary>
        /// Create a new Projection with the same formatting but
        /// filtered data
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Project a single value from a property on the subject.  Uses the
        /// accessor name as the node name by default
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public AccessorProjection<T, TValue> Value<TValue>(Expression<Func<T, TValue>> expression)
        {
            var accessor = ReflectionHelper.GetAccessor(expression);
            var value = new AccessorProjection<T, TValue>(accessor);
            if (_formatting == DisplayFormatting.UseDisplayFormatting)
            {
                value.Formatted();
            }

            value.Name(Naming.Name(accessor));

            _values.Add(value);

            return value;
        }

        public AdaptiveAccessorProjection<T> AdaptiveValue(Expression<Func<T, object>> expression)
        {
            var accessor = ReflectionHelper.GetAccessor(expression);
            var projection = new AdaptiveAccessorProjection<T>(accessor);

            _values.Add(projection);

            return projection;
        }

        /// <summary>
        /// Mix in an existing Projection definition into this one
        /// </summary>
        /// <typeparam name="TProjection"></typeparam>
        public void Include<TProjection>() where TProjection : IProjection<T>, new()
        {
            _values.Add(new DelegatingProjection<T, TProjection>());
        }

        /// <summary>
        /// Project a child node for a complex object represented by a property
        /// on the subject T type
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public ChildProjection<T, TChild> Child<TChild>(Expression<Func<T, TChild>> expression) where TChild : class
        {
            var child = new ChildProjection<T, TChild>(expression, _formatting);
            child.As<IAccessorProjection>().ApplyNaming(_naming);

            _values.Add(child);

            return child;
        }

        /// <summary>
        /// Project special leafs or nodes that don't fall into any of the 
        /// simple Property/Value patterns 
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public SingleLineExpression ForAttribute(string attributeName)
        {
            return new SingleLineExpression(attributeName, this);
        }

        /// <summary>
        /// Explicitly write values to the projected media
        /// </summary>
        /// <param name="writer"></param>
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

            /// <summary>
            /// Specify the source of the data to be added to the projection for simple
            /// key/value pairs
            /// </summary>
            /// <param name="source"></param>
            public void Use(Func<IProjectionContext<T>, object> source)
            {
                var projection = new SingleValueProjection<T>(_attributeName, source);
                _parent._values.Add(projection);
            }

            /// <summary>
            /// Specify the source of the data to be added to the projection.  Use this when
            /// you may need to add extra formatting to the raw value
            /// </summary>
            /// <typeparam name="TValue"></typeparam>
            /// <param name="expression"></param>
            /// <returns></returns>
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

                /// <summary>
                /// Explicit formatting logic
                /// </summary>
                /// <param name="source"></param>
                public void Use(Func<TValue, string> source)
                {
                    _parent.Use(context => {
                        var raw = context.Values.ValueFor(_accessor);
                        if (raw == null)
                        {
                            return string.Empty;
                        }

                        return source((TValue) raw);
                    });
                }
            }

            /// <summary>
            /// Write a nested object to the projected media using the data returned by the "source"
            /// argument.  
            /// </summary>
            /// <typeparam name="TChild"></typeparam>
            /// <param name="source"></param>
            /// <returns></returns>
            public ChildProjection<T, TChild> WriteChild<TChild>(Func<IProjectionContext<T>, TChild> source)
                where TChild : class
            {
                var child = new ChildProjection<T, TChild>(_attributeName, source, _parent._formatting);
                _parent._values.Add(child);

                return child;
            }

            /// <summary>
            /// Write a nested enumeration of nodes to the media based on the data returned by
            /// the "source" argument.
            /// </summary>
            /// <typeparam name="TElement"></typeparam>
            /// <param name="source"></param>
            /// <returns></returns>
            public EnumerableExpression<TElement> WriteEnumerable<TElement>(
                Func<IProjectionContext<T>, IEnumerable<TElement>> source)
            {
                var enumerable = new EnumerableProjection<T, TElement>
                {
                    ElementSource = source,
                    NodeName = _attributeName
                };

                _parent._values.Add(enumerable);

                return new EnumerableExpression<TElement>(enumerable);
            }
        }

        /// <summary>
        /// Write a nested enumeration of nodes to the media based on the value of the property
        /// determined by the "expression" argument
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
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

            /// <summary>
            /// Override the node name for the enumeration in the output media
            /// </summary>
            /// <param name="nodeName"></param>
            /// <returns></returns>
            public EnumerableExpression<TChild> NodeName(string nodeName)
            {
                _enumerable.NodeName = nodeName;
                return this;
            }

            /// <summary>
            /// Override the node name for each element of the enumeration within the output media
            /// </summary>
            /// <param name="leafName"></param>
            /// <returns></returns>
            public EnumerableExpression<TChild> LeafName(string leafName)
            {
                _enumerable.LeafName = leafName;
                return this;
            }

            /// <summary>
            /// Mix in a pre-built projection for each element in the enumeration
            /// </summary>
            /// <typeparam name="TProjection"></typeparam>
            /// <returns></returns>
            public EnumerableExpression<TChild> UseProjection<TProjection>()
                where TProjection : IProjection<TChild>, new()
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
            get { yield return MimeType.Json.Value; }
        }

        void DescribesItself.Describe(Description description)
        {
            description.Title = GetType().Name;
            description.ShortDescription = "Projection of " + typeof (T).FullName;
        }

        public void CamelCaseAttributeNames()
        {
            Naming = new CamelCaseNaming();
        }
    }
}