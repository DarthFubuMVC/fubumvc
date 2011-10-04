using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuMVC.Core.Resources.Media.Projections
{
    public class Projection<T> : IValueProjection<T>
    {
        private readonly IList<IValueProjection<T>> _values = new List<IValueProjection<T>>();


        void IValueProjection<T>.WriteValue(IValues<T> target, IMediaNode node)
        {
            _values.Each(x => x.WriteValue(target, node));
        }

        public AccessorProjection<T> Value(Expression<Func<T, object>> expression)
        {
            var value = new AccessorProjection<T>(expression.ToAccessor());
            _values.Add(value);

            return value;
        }
    }
}