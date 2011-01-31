using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Reflection.Expressions;

namespace FubuFastPack.Querying
{
    public class QueryService
    {
        private readonly IEnumerable<IFilterHandler> _handlers;

        public QueryService(IEnumerable<IFilterHandler> handlers)
        {
            _handlers = handlers;
        }

        private IEnumerable<IFilterHandler> allHandlers()
        {
            foreach (var filterHandler in _handlers)
            {
                yield return filterHandler;
            }

            foreach (var filterHandler in BasicFiltersCache.AllHandlers)
            {
                yield return filterHandler;
            }
        }

        public IEnumerable<OperatorKeys> FilterOptionsFor<TEntity>(Expression<Func<TEntity, object>> property)
        {
            return allHandlers().SelectMany(x => x.FilterOptionsFor(property));
        }

        public Expression<Func<T, bool>> WhereFilterFor<T>(FilterRequest<T> request)
        {
            return allHandlers().First(h => h.Matches(request.Property)).WhereFilterFor(request);
        }
    }

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

        public object GetValue()
        {
            if (PropertyType == typeof (string)) return _criteria.value;

            return _converter.FromString(_criteria.value, PropertyType);
        }
    }

    public static class BasicFiltersCache
    {
        private static readonly Func<Type, bool> numbers = type => type.IsNumeric();
        private static readonly Func<Type, bool> booleans = type => type.IsBoolean();
        private static readonly Func<Type, bool> strings = type => type == typeof (string);

        private static readonly IList<IFilterHandler> _handlers = new List<IFilterHandler>();

        static BasicFiltersCache()
        {
            filterIs<EqualsPropertyOperation>(OperatorKeys.EQUAL, numbers, strings, booleans, typeOf<Guid>());
            filterIs<NotEqualPropertyOperation>(OperatorKeys.NOTEQUAL, numbers, strings, booleans, typeOf<Guid>());
            filterIs<GreaterThanPropertyOperation>(OperatorKeys.GREATERTHAN, numbers);
            filterIs<LessThanPropertyOperation>(OperatorKeys.LESSTHAN, numbers);
            filterIs<GreaterThanOrEqualPropertyOperation>(OperatorKeys.GREATERTHANOREQUAL, numbers);
            filterIs<LessThanOrEqualPropertyOperation>(OperatorKeys.LESSTHANOREQUAL, numbers);

            filterIs<StringStartsWithPropertyOperation>(OperatorKeys.STARTSWITH, strings);
            filterIs<StringEndsWithPropertyOperation>(OperatorKeys.ENDSWITH, strings);
            filterIs<StringContainsPropertyOperation>(OperatorKeys.CONTAINS, strings);
        }

        public static IEnumerable<IFilterHandler> AllHandlers
        {
            get { return _handlers; }
        }

        private static Func<Type, bool> typeOf<T>()
        {
            return type => type == typeof (T);
        }

        private static void filterIs<T>(OperatorKeys key, params Func<Type, bool>[] typeFilters)
            where T : IPropertyOperation, new()
        {
            var handler = new BinaryFilterHandler<T>(key, typeFilters);
            _handlers.Add(handler);
        }
    }

    public interface IFilterHandler
    {
        IEnumerable<OperatorKeys> FilterOptionsFor<T>(Expression<Func<T, object>> property);
        bool Matches<T>(Expression<Func<T, object>> property);

        // TODO -- do something with ICriteria
        Expression<Func<T, bool>> WhereFilterFor<T>(FilterRequest<T> request);
    }

    public class BinaryFilterHandler<TOperation> : IFilterHandler where TOperation : IPropertyOperation, new()
    {
        private readonly OperatorKeys _key;
        private readonly Func<Type, bool>[] _typeFilters;

        public BinaryFilterHandler(OperatorKeys key, params Func<Type, bool>[] typeFilters)
        {
            _key = key;
            _typeFilters = typeFilters;
        }

        public IEnumerable<OperatorKeys> FilterOptionsFor<TEntity>(Expression<Func<TEntity, object>> property)
        {
            if (!Matches(property)) yield break;

            yield return _key;
        }

        public bool Matches<TEntity>(Expression<Func<TEntity, object>> property)
        {
            var accessor = property.ToAccessor();
            return _typeFilters.Any(f => f(accessor.PropertyType));
        }

        public Expression<Func<T, bool>> WhereFilterFor<T>(FilterRequest<T> request)
        {
            return new TOperation().GetPredicate(request.Property, request.GetValue());
        }
    }
}