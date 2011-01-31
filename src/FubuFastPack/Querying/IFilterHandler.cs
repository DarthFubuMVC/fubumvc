using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FubuFastPack.Querying
{
    public interface IFilterHandler
    {
        IEnumerable<OperatorKeys> FilterOptionsFor<T>(Expression<Func<T, object>> property);
        bool Handles<T>(FilterRequest<T> request);

        // TODO -- do something with ICriteria
        Expression<Func<T, bool>> WhereFilterFor<T>(FilterRequest<T> request);
    }
}