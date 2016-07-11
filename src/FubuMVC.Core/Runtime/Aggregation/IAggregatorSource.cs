using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FubuMVC.Core.Runtime.Aggregation
{
    public interface IAggregatorSource
    {
        Task<object> ForInputType(Type inputType);
        Task<object> ForQuery<T>(T query);
        Task<object> ForResource(Type resourceType);
        Task<object> ForAction<T>(Expression<Func<T, object>> expression);
    }
}