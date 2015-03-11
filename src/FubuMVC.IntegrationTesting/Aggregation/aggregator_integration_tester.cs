using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Runtime.Aggregation;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Aggregation
{
    [TestFixture]
    public class aggregator_integration_tester
    {
        [Test]
        public void aggregate_can_use_a_query()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<AggregationEndpoint>(x => x.get_aggregation());

                var json = _.Response.Body.ReadAsText();
                string[] values = JsonUtil.Get<string[]>(json);

                values.ShouldContain("Resource1: Jeremy Maclin");
            });
        }

        [Test]
        public void aggregate_by_resource_type()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<AggregationEndpoint>(x => x.get_aggregation());

                var json = _.Response.Body.ReadAsText();
                string[] values = JsonUtil.Get<string[]>(json);

                values.ShouldContain("I am Resource2");
            });
        }

        
        [Test]
        public void aggregate_selection_by_input_type()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<AggregationEndpoint>(x => x.get_aggregation());

                var json = _.Response.Body.ReadAsText();
                string[] values = JsonUtil.Get<string[]>(json);

                values.ShouldContain("Resource3 from input type 2");
            });
        }

        [Test]
        public void aggregate_selection_by_method()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<AggregationEndpoint>(x => x.get_aggregation());

                var json = _.Response.Body.ReadAsText();
                string[] values = JsonUtil.Get<string[]>(json);

                values.ShouldContain("Resource4 from an action identified by expression");
            });
        }

    }

    public class AggregationEndpoint
    {
        private readonly IAggregator _aggregator;

        public AggregationEndpoint(IAggregator aggregator)
        {
            _aggregator = aggregator;
        }

        public Resource1 get_query1_Name(Query1 query)
        {
            return new Resource1 {Name = query.Name};
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

        public string[] get_aggregation()
        {
            return _aggregator.Fetch(_ =>
            {
                // By an input query
                _.Query(new Query1{Name = "Jeremy Maclin"});

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

        public class Input1
        {
            
        }

        public class Resource2
        {
            public override string ToString()
            {
                return "I am Resource2";
            }
        }

        public class Input2 { }

        public class Resource3
        {
            public override string ToString()
            {
                return "Resource3 from input type 2";
            }
        }

        public class Resource4
        {
            public override string ToString()
            {
                return "Resource4 from an action identified by expression";
            }
        }

        public class Query1
        {
            public string Name { get; set; }
        }

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