using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FubuMVC.Core.Runtime.Aggregation
{
    public interface IAggregator
    {
        Task<IEnumerable<object>> Fetch(AggregateRequest request);
        Task<IEnumerable<object>> Fetch(Action<AggregateRequest> configure);
        Task<AggregationResponse> QueryAggregate(AggregatedQuery request);
    }
}