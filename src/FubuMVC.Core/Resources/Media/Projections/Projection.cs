using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuMVC.Core.Resources.Media.Projections
{
    public interface IValueProjection<T>
    {
        void WriteValue(IProjectionContext<T> target, IMediaNode node);
    }

    public class SingleValueProjection<T> : IValueProjection<T>
    {
        private string _attributeName;
        private Func<IProjectionContext<T>, object> _source;

        public SingleValueProjection(string attributeName, Func<IProjectionContext<T>, object> source)
        {
            _attributeName = attributeName;
            _source = source;
        }

        protected string attributeName
        {
            get { return _attributeName; }
            set { _attributeName = value; }
        }

        public void WriteValue(IProjectionContext<T> target, IMediaNode node)
        {
            node.SetAttribute(_attributeName, _source(target));
        }
    }

    // TODO -- add the option to make this formatted
    public class AccessorProjection<T> : SingleValueProjection<T>
    {
        public AccessorProjection(Accessor accessor) : base(accessor.Name, context => context.ValueFor(accessor))
        {
        }

        public static AccessorProjection<T> For(Expression<Func<T, object>> expression)
        {
            return new AccessorProjection<T>(expression.ToAccessor());
        }

        public AccessorProjection<T> Name(string value)
        {
            attributeName = value;
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

        public AccessorProjection<T> Value(Expression<Func<T, object>> expression)
        {
            var value = new AccessorProjection<T>(expression.ToAccessor());
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
        }
    }
}