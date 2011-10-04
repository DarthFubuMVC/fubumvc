using System;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuMVC.Core.Resources.Media.Projections
{
    public class AccessorProjection<T> : IValueProjection<T>
    {
        private readonly Accessor _accessor;

        private string _projectedNodeName;

        public AccessorProjection(Accessor accessor)
        {
            _accessor = accessor;
            Name(accessor.Name);
        }

        public void WriteValue(IValues<T> target, IMediaNode node)
        {
            var value = target.ValueFor(_accessor);
            node.SetAttribute(Name(), value);
        }

        public static AccessorProjection<T> For(Expression<Func<T, object>> expression)
        {
            return new AccessorProjection<T>(expression.ToAccessor());
        }

        public AccessorProjection<T> Name(string value)
        {
            _projectedNodeName = value;
            return this;
        }

        public string Name()
        {
            return _projectedNodeName;
        }
    }
}