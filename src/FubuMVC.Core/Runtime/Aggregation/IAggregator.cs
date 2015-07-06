using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Runtime.Aggregation
{
    public interface IAggregator
    {
        IEnumerable<object> Fetch(AggregateRequest request);
        IEnumerable<object> Fetch(Action<AggregateRequest> configure);
        AggregationResponse QueryAggregate(AggregatedQuery request);
    }
}