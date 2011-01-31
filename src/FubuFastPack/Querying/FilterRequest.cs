using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;

namespace FubuFastPack.Querying
{
    public class FilterRequest<T>
    {
        private readonly Accessor _accessor;
        private readonly IObjectConverter _converter;
        private readonly Criteria _criteria;
        private readonly Expression<Func<T, object>> _property;

        public FilterRequest(Criteria criteria, IObjectConverter converter, Expression<Func<T, object>> property)
        {
            _criteria = criteria;
            _converter = converter;
            _property = property;
            _accessor = _property.ToAccessor();
        }

        public Type PropertyType
        {
            get { return _accessor.PropertyType; }
        }

        public Expression<Func<T, object>> Property
        {
            get { return _property; }
        }

        public string Operator
        {
            get { return _criteria.op; }
        }

        public object GetValue()
        {
            if (PropertyType == typeof (string)) return _criteria.value;

            return _converter.FromString(_criteria.value, PropertyType);
        }
    }
}