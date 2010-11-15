using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.DSL;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Expressions
{
    public class OredevFubuRegistry : FubuRegistry
    {
        public OredevFubuRegistry()
        {
            Route("route/to/this")
                .Calls<when_explicitly_registering_a_route.InputController>(x => x.DoSomething(null))
                .OutputToJson();
        }
    }


    [TestFixture]
    public class when_explicitly_configuring_a_route
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var config = new ExplicitRouteConfiguration("some/pattern");
            config.Chain();
            _config = config;
            _graph = new FubuRegistry(registry =>
            {
                

                registry.Route<InputModel>("some/pattern")
                    .Calls<InputController>(c => c.DoSomething(null)).OutputToJson();
            })
                    .BuildGraph();

            _graph.Behaviors.ShouldHaveCount(1);
            _config.Configure(_graph);
        }

        #endregion

        private IConfigurationAction _config;
        private BehaviorGraph _graph;

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
        public void should_add_new_behavior_node_to_graph()
        {
            _graph.Behaviors.ShouldHaveCount(2);

            var visitor = new BehaviorVisitor(new NulloConfigurationObserver(), "");
            visitor.Filters += chain => !chain.ContainsCall(call => call.InputType() == typeof (InputModel));
            visitor.Actions += chain => chain.ShouldHaveCount(0);

            _graph.VisitBehaviors(visitor);
        }
    }
}