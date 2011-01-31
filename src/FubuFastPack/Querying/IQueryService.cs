using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FubuFastPack.Querying
{
    public interface IQueryService
    {
        IEnumerable<OperatorKeys> FilterOptionsFor<TEntity>(Expression<Func<TEntity, object>> property);
        Expression<Func<T, bool>> WhereFilterFor<T>(FilterRequest<T> request);
    }
}