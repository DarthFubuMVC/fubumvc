using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuLocalization;

namespace FubuFastPack.Querying
{
    // TODO -- do something with ICriteria
    public interface IFilterHandler
    {
        IEnumerable<StringToken> FilterOptionsFor<T>(Accessor accessor);
        bool Handles<T>(FilterRequest<T> request);
        Expression<Func<T, bool>> WhereFilterFor<T>(FilterRequest<T> request);

        FilterRule ToFilterRule<T>(FilterRequest<T> request);
    }
}