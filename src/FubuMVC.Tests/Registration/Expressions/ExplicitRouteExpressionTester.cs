using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Tests.Registration.Utilities;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Expressions
{
    [TestFixture, Explicit]
    public class when_explicitly_registering_a_route
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = new FubuRegistry(x =>
            {
                x.Route<Registration.InputModel>("/area/sub/{Name}/{Age}")
                    .Calls<InputController>(c => c.DoSomething(null)).OutputToJson();
            }).BuildGraph();


            route = graph.RoutesFor<Registration.InputModel>().First();
        }

        #endregion

        private BehaviorGraph graph;
        private IRouteDefinition route;

        public class InputModel
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public int Age2 { get; set; }
        }

        public class OutputModel
        {
        }

        public class InputController
        {
            public OutputModel DoSomething(InputModel input)
            {
                return new OutputModel();
            }
        }

        [Test]
        public void should_have_the_route()
        {
            route.ShouldNotBeNull();
        }

        [Test]
        public void should_only_be_one_route()
        {
            graph.RoutesFor<Registration.InputModel>().Count().ShouldEqual(1);
        }

        [Test]
        public void the_constructed_behavior_chain_should_be_a_call_to_the_action_and_then_to_json()
        {
            ActionCallSpec spec = ActionCallSpec.For<InputController>(c => c.DoSomething(null));
            spec.Json<OutputModel>();

            spec.Verify(graph.BehaviorFor(route));
        }
    }
}