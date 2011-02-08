using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace FubuFastPack.Querying
{
    // TODO -- do something with ICriteria
    public interface IFilterHandler
    {
        IEnumerable<OperatorKeys> FilterOptionsFor<T>(Accessor accessor);
        bool Handles<T>(FilterRequest<T> request);
        Expression<Func<T, bool>> WhereFilterFor<T>(FilterRequest<T> request);
    }
}