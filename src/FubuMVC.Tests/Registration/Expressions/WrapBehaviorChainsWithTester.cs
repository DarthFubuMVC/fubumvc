using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;
using FubuMVC.StructureMap;
using FubuMVC.Tests.Diagnostics;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
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
                x.Route("area/sub/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                x.Route("area/sub2/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                x.Route("area/sub3/{Name}/{Age}")
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
            BehaviorGraph graph = registry.BuildLightGraph();

            graph.Behaviors.Count().ShouldEqual(3);
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
            var container = new Container(x =>
            {
                x.For<IStreamingData>().Use(MockRepository.GenerateMock<IStreamingData>());
                x.For<IHttpWriter>().Use(new NulloHttpWriter());
                x.For<ICurrentChain>().Use(new CurrentChain(null, null));
                x.For<ICurrentHttpRequest>().Use(new StubCurrentHttpRequest("http://server"));
            });

            FubuApplication.For(() => registry).StructureMap(container).Bootstrap();

            container.Model.InstancesOf<IActionBehavior>().Count().ShouldBeGreaterThan(3);

            // The InputBehavior is first
            container.GetAllInstances<IActionBehavior>().Each(x =>
            {
                if (x.GetType().Closes(typeof(InputBehavior<>)))
                {
                    x.As<BasicBehavior>().InsideBehavior.ShouldBeOfType<FakeUnitOfWorkBehavior>().Inner.ShouldNotBeNull();
                }
                else
                {
                    x.ShouldBeOfType<FakeUnitOfWorkBehavior>().Inner.ShouldNotBeNull();
                }

                
            });
        }
    }
}