using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuMVC.Core.Projections
{
    public class Projection<T> : IValueProjection
    {
        private readonly IList<IValueProjection> _values = new List<IValueProjection>();

        public AccessorProjection Value(Expression<Func<T, object>> expression)
        {
            var value = new AccessorProjection(expression.ToAccessor());
            _values.Add(value);

            return value;
        }



        void IValueProjection.WriteValue(IProjectionTarget target, IMediaNode node)
        {
            _values.Each(x => x.WriteValue(target, node));
        }
    }
}