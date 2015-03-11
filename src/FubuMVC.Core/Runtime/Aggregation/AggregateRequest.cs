using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FubuMVC.Core.Runtime.Aggregation
{
    public class AggregateRequest
    {
        private readonly IList<Func<IAggregatorSource, object>> _sources = new List<Func<IAggregatorSource, object>>();

        internal IEnumerable<object> Aggregate(IAggregatorSource source)
        {
            return _sources.Select(x => x(source)).Where(x => x != null).ToArray();
        }

        public void Resource<T>()
        {
            _sources.Add(x => x.ForResource(typeof(T)));
        }

        public void Input<T>()
        {
            _sources.Add(x => x.ForInputType(typeof(T)));
        }

        public void Query<T>(T input)
        {
            _sources.Add(x => x.ForQuery(input));
        }

        public void Action<T>(Expression<Func<T, object>> expression)
        {
            _sources.Add(x => x.ForAction(expression));
        }
    }
}