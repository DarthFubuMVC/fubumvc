using System.Linq;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.StructureMap;
using FubuMVC.Tests.Diagnostics;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace FubuMVC.Tests.Registration.Expressions
{
    [TestFixture]
    public class ConditionallyWrapBehaviorChainsWithTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            registry = new FubuRegistry(x =>
            {
                // Tell FubuMVC to wrap the behavior chain for each
                // RouteHandler with the "FakeUnitOfWorkBehavior"
                // Kind of like a global [ActionFilter] in MVC
                x.Policies.ConditionallyWrapBehaviorChainsWith<FakeUnitOfWorkBehavior>(
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

            _graph = registry.BuildGraph();
        }

        #endregion

        private FubuRegistry registry;
        private BehaviorGraph _graph;

        public class FakeUnitOfWorkBehavior : IActionBehavior
        {
            private readonly IActionBehavior _inner;

            public FakeUnitOfWorkBehavior(IActionBehavior inner)
            {
                _inner = inner;
            }

            public IActionBehavior Inner
            {
                get { return _inner; }
            }

            public void Invoke()
            {
            }

            public void InvokePartial()
            {
            }
        }

        [Test]
        public void other_actions_should_not_be_wrapped()
        {
            var visitor = new BehaviorVisitor(new NulloConfigurationObserver(), "");
            visitor.Filters += chain => chain.Calls.Any(call => call.Method.Name != "SomeAction");
            visitor.Filters += chain => chain.Calls.Any(call => call.HandlerType != typeof (AssetWriter));

            visitor.Actions += chain => chain.Top.Next.ShouldBeOfType<ActionCall>();

            _graph.VisitBehaviors(visitor);
        }

        [Test]
        public void someaction_call_should_be_wrapped()
        {
            var visitor = new BehaviorVisitor(new NulloConfigurationObserver(), "");
            visitor.Filters += chain => chain.Calls.Any(call => call.Method.Name == "SomeAction");
            visitor.Actions += chain =>
            {
                var wrapper = chain.Top.Next.ShouldBeOfType<Wrapper>();
                wrapper.BehaviorType.ShouldEqual(typeof (FakeUnitOfWorkBehavior));
                wrapper.Next.ShouldBeOfType<ActionCall>();
            };

            _graph.VisitBehaviors(visitor);
        }
    }

    [TestFixture]
    public class container_facility_smoke_test
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            registry = new FubuRegistry(x =>
            {
                // Tell FubuMVC to wrap the behavior chain for each
                // RouteHandler with the "FakeUnitOfWorkBehavior"
                // Kind of like a global [ActionFilter] in MVC
                x.Policies.ConditionallyWrapBehaviorChainsWith
                    <ConditionallyWrapBehaviorChainsWithTester.FakeUnitOfWorkBehavior>(
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
        }

        #endregion

        private FubuRegistry registry;

        [Test]
        public void hydrate_through_container_facility_smoke_test()
        {
            var container = new Container(x =>
            {
                x.For<IStreamingData>().Use(MockRepository.GenerateMock<IStreamingData>());
                x.For<IHttpWriter>().Use(new NulloHttpWriter());
                x.For<ICurrentChain>().Use(new CurrentChain(null, null));
                x.For<ICurrentHttpRequest>().Use(new StubCurrentHttpRequest("http://server"));
            });

            FubuApplication.For(() => registry).StructureMap(container).Bootstrap();

            container.Model.InstancesOf<IActionBehavior>().Count().ShouldBeGreaterThan(3);

            var behaviors = container.GetAllInstances<IActionBehavior>().ToArray();
            
            // The first behavior is an InputBehavior
            behaviors[0].As<BasicBehavior>().InsideBehavior.ShouldBeOfType<ConditionallyWrapBehaviorChainsWithTester.FakeUnitOfWorkBehavior>().Inner.
                ShouldNotBeNull();
            behaviors[1].As<BasicBehavior>().InsideBehavior.ShouldNotBeOfType<ConditionallyWrapBehaviorChainsWithTester.FakeUnitOfWorkBehavior>();
            behaviors[2].As<BasicBehavior>().InsideBehavior.ShouldNotBeOfType<ConditionallyWrapBehaviorChainsWithTester.FakeUnitOfWorkBehavior>();
        }
    }
}