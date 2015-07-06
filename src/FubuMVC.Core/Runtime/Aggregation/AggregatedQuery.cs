using System.Collections.Generic;

namespace FubuMVC.Core.Runtime.Aggregation
{
    public class AggregatedQuery
    {
        private readonly List<ClientQuery> _queries = new List<ClientQuery>();

        public ClientQuery[] queries
        {
            get { return _queries.ToArray(); }
            set
            {
                _queries.Clear();
                if (value != null) _queries.AddRange(value);
            }
        }

        public void AddQuery<T>(T input)
        {
            _queries.Add(new ClientQuery
            {
                query = input,
                type = typeof(T).GetMessageName()
            });
        }

        public void Resource<T>()
        {
            _queries.Add(new ClientQuery{type = typeof(T).GetMessageName()});
        }
    }
}