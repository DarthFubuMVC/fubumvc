using System.IO;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Json;
using FubuMVC.Core.Runtime.Aggregation;
using Shouldly;
using HtmlTags;
using Newtonsoft.Json;
using NUnit.Framework;
using StructureMap;

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
                var values = JsonUtil.Get<string[]>(json);

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
                var values = JsonUtil.Get<string[]>(json);

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
                var values = JsonUtil.Get<string[]>(json);

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
                var values = JsonUtil.Get<string[]>(json);

                values.ShouldContain("Resource4 from an action identified by expression");
            });
        }


        [Test]
        public void can_still_call_a_data_action_independently()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Input(new AggregationEndpoint.Query1 {Name = "Joe"});

                var json = _.Response.Body.ReadAsText();

                var resource = JsonUtil.Get<AggregationEndpoint.Resource1>(json);
                resource.Name.ShouldBe("Joe");
            });
        }

        [Test]
        public void client_message_cache_knows_its_chains()
        {
            var cache = TestHost.Service<IClientMessageCache>();
            cache.AllClientMessages()
                .ShouldHaveTheSameElementsAs(
                    new ClientMessagePath
                    {
                        Message = "query-1",
                        InputType = typeof (AggregationEndpoint.Query1),
                        ResourceType = typeof (AggregationEndpoint.Resource1)
                    },
                    new ClientMessagePath
                    {
                        Message = "resource-2",
                        InputType = null,
                        ResourceType = typeof (AggregationEndpoint.Resource2)
                    },
                    new ClientMessagePath
                    {
                        Message = "input-2",
                        InputType = typeof (AggregationEndpoint.Input2),
                        ResourceType = typeof (AggregationEndpoint.Resource3)
                    },
                    new ClientMessagePath
                    {
                        Message = "resource-4",
                        InputType = null,
                        ResourceType = typeof (AggregationEndpoint.Resource4)
                    }
                );
        }

        [Test]
        public void find_chain_by_message_type()
        {
            var cache = TestHost.Service<IClientMessageCache>();
            cache.FindChain("query-1").InputType().ShouldBe(typeof (AggregationEndpoint.Query1));
            cache.FindChain("resource-4").ResourceType().ShouldBe(typeof (AggregationEndpoint.Resource4));
        }

        [Test]
        public void aggregated_request_with_an_input_type()
        {
            var container = TestHost.Service<IContainer>();

            using (var nested = container.GetNestedContainer())
            {
                var aggregator = nested.GetInstance<Aggregator>();


                var response = aggregator.ExecuteQuery(new ClientQuery
                {
                    query = new AggregationEndpoint.Query1 {Name = "Justin Houston"},
                    type = typeof (AggregationEndpoint.Query1).GetMessageName()
                });

                response.request.ShouldBe("query-1");
                response.type.ShouldBe("resource-1");
                response.result.ShouldBeOfType<AggregationEndpoint.Resource1>()
                    .Name.ShouldBe("Justin Houston");
            }
        }

        [Test]
        public void aggregate_request_through_the_initial_endpoint()
        {
            var container = TestHost.Service<IContainer>();

            using (var nested = container.GetNestedContainer())
            {
                var aggregator = nested.GetInstance<Aggregator>();

                var query = new AggregatedQuery();
                query.AddQuery(new AggregationEndpoint.Query1 {Name = "Jeremy Maclin"});
                query.Resource<AggregationEndpoint.Resource2>();
                query.AddQuery(new AggregationEndpoint.Input2());
                query.Resource<AggregationEndpoint.Resource4>();

                var aggregatedResponse = aggregator.QueryAggregate(query);

                aggregatedResponse.responses[0].result.ShouldBeOfType<AggregationEndpoint.Resource1>()
                    .Name.ShouldBe("Jeremy Maclin");

                aggregatedResponse.responses[1].result.ShouldBeOfType<AggregationEndpoint.Resource2>();
                aggregatedResponse.responses[2].result.ShouldBeOfType<AggregationEndpoint.Resource3>();
                aggregatedResponse.responses[3].result.ShouldBeOfType<AggregationEndpoint.Resource4>();
            }
        }

        [Test]
        public void use_aggregated_query_reader()
        {
            var query = new AggregatedQuery();
            query.AddQuery(new AggregationEndpoint.Query1 {Name = "Jeremy Maclin"});
            query.Resource<AggregationEndpoint.Resource2>();
            query.AddQuery(new AggregationEndpoint.Input2());
            query.Resource<AggregationEndpoint.Resource4>();

            var json = JsonUtil.ToJson(query);

            var messageTypes = TestHost.Service<IClientMessageCache>();

            var readQuery = new AggregatedQueryReader().Read(new JsonSerializer(), messageTypes, json);

            readQuery.ShouldNotBeNull();
            readQuery.queries[0].type.ShouldBe("query-1");
            readQuery.queries[0].query.ShouldBeOfType<AggregationEndpoint.Query1>()
                .Name.ShouldBe("Jeremy Maclin");

            readQuery.queries[1].type.ShouldBe("resource-2");
            readQuery.queries[2].type.ShouldBe("input-2");
            readQuery.queries[3].query.ShouldBeNull();
        }

        [Test]
        public void run_aggregated_query_through_http_endpoint()
        {
            var query = new AggregatedQuery();
            query.AddQuery(new AggregationEndpoint.Query1 {Name = "Jeremy Maclin"});
            query.Resource<AggregationEndpoint.Resource2>();
            query.AddQuery(new AggregationEndpoint.Input2());
            query.Resource<AggregationEndpoint.Resource4>();

            TestHost.Scenario(_ =>
            {
                _.Post.Json(query);

                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("application/json");

                var json = _.Response.Body.ReadAsText();

                var response =
                    new JsonSerializer().Deserialize<AggregationResponse>(new JsonTextReader(new StringReader(json)));

                response.responses.Count().ShouldBe(4);
                response.responses.Select(x => x.type)
                    .ShouldHaveTheSameElementsAs("resource-1", "resource-2", "resource-3", "resource-4");
            });
        }

        [Test]
        public void passes_corellation_id_through_http_endpoint()
        {
            var data = JsonUtil.ToJson(new
            {
                queries = new[]
                {
                    new
                    {
                        type = "query-1",
                        query = new
                        {
                            name = "Joe"
                        },
                        correlationId = "123"
                    }
                }
            });

            var messageTypes = TestHost.Service<IClientMessageCache>();

            var readQuery = new AggregatedQueryReader().Read(new JsonSerializer(), messageTypes, data);

            TestHost.Scenario(_ =>
            {
                _.Post.Json(readQuery);

                _.StatusCodeShouldBeOk();
                _.ContentTypeShouldBe("application/json");

                var json = _.Response.Body.ReadAsText();

                var response =
                    new JsonSerializer().Deserialize<AggregationResponse>(new JsonTextReader(new StringReader(json)));

                response.responses.Count().ShouldBe(1);
                response.responses[0].correlationId.ShouldBe("123");
            });
        }

        [Test]
        public void reads_corellation_id_in_aggregated_query_reader()
        {
            var json = JsonUtil.ToJson(new
            {
                queries = new[]
                {
                    new
                    {
                        type = "query-1",
                        query = new
                        {
                            name = "Joe"
                        },
                        correlationId = "123"
                    }
                }
            });

            var messageTypes = TestHost.Service<IClientMessageCache>();

            var readQuery = new AggregatedQueryReader().Read(new JsonSerializer(), messageTypes, json);

            readQuery.ShouldNotBeNull();
            readQuery.queries[0].type.ShouldBe("query-1");
            readQuery.queries[0].query.ShouldBeOfType<AggregationEndpoint.Query1>()
                .Name.ShouldBe("Joe");
            readQuery.queries[0].correlationId.ShouldBe("123");
        }

        [Test]
        public void reads_query_without_corellation_id_in_aggregated_query_reader()
        {
            var json = JsonUtil.ToJson(new
            {
                queries = new[]
                {
                    new
                    {
                        type = "query-1",
                        query = new
                        {
                            name = "Joe"
                        }
                    }
                }
            });

            var messageTypes = TestHost.Service<IClientMessageCache>();

            var readQuery = new AggregatedQueryReader().Read(new JsonSerializer(), messageTypes, json);

            readQuery.ShouldNotBeNull();
            readQuery.queries[0].type.ShouldBe("query-1");
            readQuery.queries[0].query.ShouldBeOfType<AggregationEndpoint.Query1>()
                .Name.ShouldBe("Joe");
            readQuery.queries[0].correlationId.ShouldBeNull();
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
                _.Query(new Query1 {Name = "Jeremy Maclin"});

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
        public class Input2
        {
        }

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

