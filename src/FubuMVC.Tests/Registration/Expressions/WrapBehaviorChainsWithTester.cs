using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.StructureMap;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.Registration.Expressions
{
    [TestFixture]
    public class WrapBehaviorChainsWithTester
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
                x.Policies.WrapBehaviorChainsWith<FakeUnitOfWorkBehavior>();

                // Explicit junk you would only do for exception cases to
                // override the conventions
                x.Route<InputModel>("area/sub/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                x.Route<InputModel>("area/sub2/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                x.Route<InputModel>("area/sub3/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();
            });
        }

        #endregion

        private FubuRegistry registry;

        public class FakeUnitOfWorkBehavior : IActionBehavior
        {
            private readonly IActionBehavior _inner;

            public FakeUnitOfWorkBehavior(IActionBehavior inner)
            {
                _inner = inner;
            }

            public IActionBehavior Inner { get { return _inner; } }

            public void Invoke()
            {
            }

            public void InvokePartial()
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void all_behaviors_chains_should_start_with_the_declared_behavior()
        {
            BehaviorGraph graph = registry.BuildGraph();

            graph.BehaviorChainCount.ShouldEqual(3);
            var visitor = new BehaviorVisitor(new NulloConfigurationObserver(), "");
            visitor.Actions += chain =>
            {
                var wrapper = chain.Top.ShouldBeOfType<Wrapper>();
                wrapper.BehaviorType.ShouldEqual(typeof(FakeUnitOfWorkBehavior));
                wrapper.Next.ShouldBeOfType<ActionCall>();
            };

            graph.VisitBehaviors(visitor);

        }

        [Test]
        public void hydrate_through_container_facility_smoke_test()
        {
            var container = new Container();

            new StructureMapBootstrapper(container, registry).Bootstrap(new RouteCollection());

            container.Model.InstancesOf<IActionBehavior>().Count().ShouldEqual(3);

            container.GetAllInstances<IActionBehavior>().Each(
                x => { x.ShouldBeOfType<FakeUnitOfWorkBehavior>().Inner.ShouldNotBeNull(); });
        }
    }
}