using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuLocalization;
using Microsoft.Practices.ServiceLocation;

namespace FubuFastPack.Querying
{
    public interface IQueryService
    {
        IEnumerable<StringToken> FilterOptionsFor<TEntity>(Accessor accessor);
        Expression<Func<T, bool>> WhereFilterFor<T>(FilterRequest<T> request);
        FilterRule FilterRuleFor<T>(FilterRequest<T> request);
    }
}