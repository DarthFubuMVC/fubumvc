using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuCore.Reflection.Expressions;
using FubuLocalization;

namespace FubuFastPack.Querying
{
    public class BinaryFilterHandler<TOperation> : IFilterHandler where TOperation : IPropertyOperation, new()
    {
        private readonly OperatorKeys _key;
        private readonly Func<Type, bool>[] _typeFilters;

        public BinaryFilterHandler(OperatorKeys key, params Func<Type, bool>[] typeFilters)
        {
            _key = key;
            _typeFilters = typeFilters;
        }

        public IEnumerable<StringToken> FilterOptionsFor<TEntity>(Accessor accessor)
        {
            if (!Matches(accessor)) yield break;

            yield return _key;
        }

        public bool Handles<T>(FilterRequest<T> request)
        {
            return _key.Key == request.Operator && Matches(request.Accessor);
        }

        public Expression<Func<T, bool>> WhereFilterFor<T>(FilterRequest<T> request)
        {
            return new TOperation().GetPredicate(request.Property, request.GetValue());
        }

        public FilterRule ToFilterRule<T>(FilterRequest<T> request)
        {
            return new FilterRule(){
                Accessor = request.Accessor,
                Operator = _key,
                Value = request.GetValue()
            };
        }

        public bool Matches(Accessor accessor)
        {
            return _typeFilters.Any(f => f(accessor.PropertyType));
        }
    }
}