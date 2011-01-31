using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FubuFastPack.Querying
{
    public class QueryService : IQueryService
    {
        private readonly IEnumerable<IFilterHandler> _handlers;

        public QueryService(IEnumerable<IFilterHandler> handlers)
        {
            _handlers = handlers;
        }

        public IEnumerable<OperatorKeys> FilterOptionsFor<TEntity>(Expression<Func<TEntity, object>> property)
        {
            return allHandlers().SelectMany(x => x.FilterOptionsFor(property));
        }

        public Expression<Func<T, bool>> WhereFilterFor<T>(FilterRequest<T> request)
        {
            return allHandlers().First(h => h.Handles(request)).WhereFilterFor(request);
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
    }
}