using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuLocalization;

namespace FubuFastPack.Querying
{
    public class FilterRequest<T>
    {
        private readonly Accessor _accessor;
        private readonly IObjectConverter _converter;
        private readonly Criteria _criteria;
        private readonly Expression<Func<T, object>> _property;

        public static FilterRequest<T> For(Expression<Func<T, object>> property, StringToken op, string value)
        {
            var criteria = Criteria.For<T>(property, op.Key, value);
            return new FilterRequest<T>(criteria, new ObjectConverter(), property);
        }

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

        public Accessor Accessor
        {
            get { return _accessor; }
        }

        public TArg GetValueAs<TArg>()
        {
            return _converter.FromString<TArg>(_criteria.value);
        }

        public object GetValue()
        {
            if (PropertyType == typeof (string)) return _criteria.value;

            return _converter.FromString(_criteria.value, PropertyType);
        }
    }
}