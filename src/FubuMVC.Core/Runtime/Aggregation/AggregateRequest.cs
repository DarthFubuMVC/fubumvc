using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FubuMVC.Core.Runtime.Aggregation
{
    public class AggregateRequest
    {
        private readonly IList<Func<IAggregatorSource, Task<object>>> _sources = new List<Func<IAggregatorSource, Task<object>>>();

        internal async Task<IEnumerable<object>> Aggregate(IAggregatorSource source)
        {
            var answers = new object[_sources.Count];

            for (int i = 0; i < _sources.Count; i++)
            {
                answers[i] = await _sources[i](source).ConfigureAwait(false);
            }


            return answers.Where(x => x != null).ToArray();
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