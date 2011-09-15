using System;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuMVC.Core.Projections
{
    public class AccessorProjection : IValueProjection
    {
        private readonly Accessor _accessor;
        
        public static AccessorProjection For<T>(Expression<Func<T, object>> expression)
        {
            return new AccessorProjection(expression.ToAccessor());
        }

        public AccessorProjection(Accessor accessor)
        {
            _accessor = accessor;
            Name(accessor.Name);
        }

        private string _projectedNodeName;

        public AccessorProjection Name(string value)
        {
            _projectedNodeName = value;
            return this;
        }

        public string Name()
        {
            return _projectedNodeName;
        }

        public void WriteValue(IProjectionTarget target, IMediaNode node)
        {
            var value = target.ValueFor(_accessor);
            node.SetAttribute(Name(), value);
        }
    }
}