using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuLocalization;

namespace FubuFastPack.Querying
{
    public class StringFilterHandler : IFilterHandler
    {
        private readonly OperatorKeys _key;
        private readonly MethodInfo _stringMethod;

        public StringFilterHandler(OperatorKeys key, Expression<Func<string, bool>> expression)
        {
            _key = key;
            _stringMethod = ReflectionHelper.GetMethod(expression);
        }

        public IEnumerable<StringToken> FilterOptionsFor<T>(Accessor accessor)
        {
            if (accessor.PropertyType != typeof(string)) yield break;

            yield return _key;
        }

        public bool Handles<T>(FilterRequest<T> request)
        {
            return _key.Key == request.Operator && request.PropertyType == typeof(string);
        }

        public Expression<Func<T, bool>> WhereFilterFor<T>(FilterRequest<T> request)
        {
            var memberExpression = request.Property.GetMemberExpression(true);
            var constantExpression = Expression.Constant(request.GetValue());

            var expression = Expression.Call(memberExpression, _stringMethod, constantExpression);

            var parameterExpression = Expression.Parameter(typeof (T), "target");

            return Expression.Lambda<Func<T, bool>>(expression, parameterExpression);
        }

        public FilterRule ToFilterRule<T>(FilterRequest<T> request)
        {
            return new FilterRule(){
                Accessor = request.Accessor,
                Operator = _key,
                Value = request.GetValue()
            };
        }
    }
}