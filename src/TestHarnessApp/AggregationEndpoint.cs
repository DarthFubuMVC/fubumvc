using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Runtime.Aggregation;

namespace TestHarnessApp
{
    public class AggregationEndpoint
    {
        private readonly IAggregator _aggregator;

        public AggregationEndpoint(IAggregator aggregator)
        {
            _aggregator = aggregator;
        }

        public Resource1 get_query1_Name(Query1 query)
        {
            return new Resource1 { Name = query.Name };
        }

        public Resource2 get_second_resource()
        {
            return new Resource2();
        }

        public Resource3 get_third_resource(Input2 input)
        {
            return new Resource3();
        }

        public Resource4 get_fourth_resource()
        {
            return new Resource4();
        }

        public AggregationResponse post_aggregated_query(AggregatedQuery query)
        {
            return _aggregator.QueryAggregate(query);
        }

        public string[] get_aggregation()
        {
            // The call to IAggregator.Fetch() will return an array
            // of objects. My assumption now is that you'd do this in
            // the main endpoint method, tack the data store data on
            // to the view model sent to the main spark view, and
            // render it to the view with a page helper there.
            // This is so common now that I think we put a json variable
            // helper into FubuMVC.Core. 
            return _aggregator.Fetch(_ =>
            {
                // By an input query
                _.Query(new Query1 { Name = "Jeremy Maclin" });

                // By the resource type
                _.Resource<Resource2>();

                // By the input type as a marker
                _.Input<Input2>();

                // By action method
                _.Action<AggregationEndpoint>(x => x.get_fourth_resource());
            })
            .Select(x => x.ToString())
            .ToArray();
        }

        [ClientMessage]
        public class Input1
        {

        }

        [ClientMessage]
        public class Resource2
        {
            public override string ToString()
            {
                return "I am Resource2";
            }
        }

        [ClientMessage]
        public class Input2 { }

        [ClientMessage]
        public class Resource3
        {
            public override string ToString()
            {
                return "Resource3 from input type 2";
            }
        }

        [ClientMessage]
        public class Resource4
        {
            public override string ToString()
            {
                return "Resource4 from an action identified by expression";
            }
        }

        [ClientMessage]
        public class Query1
        {
            public string Name { get; set; }
        }

        [ClientMessage]
        public class Resource1
        {
            public string Name { get; set; }

            public override string ToString()
            {
                return "Resource1: " + Name;
            }
        }
    }
}