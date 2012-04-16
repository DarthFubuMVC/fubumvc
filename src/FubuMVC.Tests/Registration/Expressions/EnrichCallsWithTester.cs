using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Expressions
{
    [TestFixture]
    public class EnrichCallsWithTester
    {
        [SetUp]
        public void SetUp()
        {
            registry = new FubuRegistry(x =>
            {
                

                // Tell FubuMVC to enrich the behavior chain for each
                // RouteHandler with the "FakeUnitOfWorkBehavior"
                // Kind of like a global [ActionFilter] in MVC
                x.Policies.EnrichCallsWith<FakeUnitOfWorkBehavior>(
                    call => call.Method.Name == "SomeAction");

                // Explicit junk you would only do for exception cases to
                // override the conventions
                x.Route("area/sub/{Name}/{Age}")
                    .Calls<TestController>(c => c.SomeAction(null)).OutputToJson();

                x.Route("area/sub2/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                x.Route("area/sub3/{Name}/{Age}")
                    .Calls<TestController>(c => c.ThirdAction(null)).OutputToJson();
            });

            _graph = registry.BuildLightGraph();
        }

        private FubuRegistry registry;
        private BehaviorGraph _graph;

        public class FakeUnitOfWorkBehavior : IActionBehavior
        {
            private readonly IActionBehavior _inner;
            public FakeUnitOfWorkBehavior(IActionBehavior inner) { _inner = inner; }
            public IActionBehavior Inner { get { return _inner; } }
            public void Invoke() { }
            public void InvokePartial() { }
        }

        [Test]
        public void someaction_call_should_be_enriched()
        {
            var visitor = new BehaviorVisitor(new NulloConfigurationObserver(), "");
            visitor.Filters += chain => chain.Calls.Any(call => call.Method.Name == "SomeAction");
            visitor.Actions += chain =>
            {
                var wrapper = chain.Top.Next.ShouldBeOfType<Wrapper>();
                wrapper.BehaviorType.ShouldEqual(typeof(FakeUnitOfWorkBehavior));
                wrapper.Previous.ShouldBeOfType<ActionCall>();
            };

            _graph.VisitBehaviors(visitor);
        }

        [Test]
        public void other_actions_should_not_be_enriched()
        {
            var visitor = new BehaviorVisitor(new NulloConfigurationObserver(), "");
            visitor.Filters += chain => chain.Calls.Any(call => call.Method.Name != "SomeAction");
            visitor.Filters += chain => chain.Calls.Any(call => call.HandlerType != typeof(ContentWriter));

            visitor.Actions += chain =>
                                   {
                                       chain.Top.ShouldBeOfType<ActionCall>();
                                       chain.Top.Next.ShouldBeOfType<OutputNode>();
                                   };

            _graph.VisitBehaviors(visitor);
        }
    }
}