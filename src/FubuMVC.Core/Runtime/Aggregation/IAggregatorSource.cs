using System;
using System.Linq.Expressions;

namespace FubuMVC.Core.Runtime.Aggregation
{
    public interface IAggregatorSource
    {
        object ForInputType(Type inputType);
        object ForQuery<T>(T query);
        object ForResource(Type resourceType);
        object ForAction<T>(Expression<Func<T, object>> expression);
    }
}