using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Routing;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Routing;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Handlers;
using FubuMVC.Core.Security;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Routing
{
    [TestFixture]
    public class StandardRoutePolicyTester
    {
        private IEnumerable<Guid> _actionIds;
        private IEnumerable<Route> _routes;
        private IServiceFactory theFactory;

        [SetUp]
        public void SetUp()
        {
            theFactory = MockRepository.GenerateMock<IServiceFactory>();

            var graph = setupActions();
            _actionIds = graph.Actions().Select(x => x.ParentChain().UniqueId);
            _routes = new StandardRoutePolicy().BuildRoutes(graph, theFactory).Cast<Route>();
        }

        [Test]
        public void DetermineInvoker_for_asynchronous_actions()
        {
            var chain = BehaviorChain.For<Action3>(x => x.M1Async());
            chain.IsAsynchronous().ShouldBeTrue();

            StandardRoutePolicy.DetermineInvoker(theFactory, chain).ShouldBeOfType<AsyncBehaviorInvoker>();
        }

        [Test]
        public void DetermineInvoker_for_synchronous_actions()
        {
            var chain = BehaviorChain.For<Action2>(x => x.M1());
            chain.IsAsynchronous().ShouldBeFalse();

            StandardRoutePolicy.DetermineInvoker(theFactory, chain).ShouldBeOfType<BehaviorInvoker>();
        }

        [Test]
        public void DetermineHandlerSource_for_synchronous_and_no_session()
        {
            var chain = BehaviorChain.For<Action2>(x => x.M1());
            chain.IsAsynchronous().ShouldBeFalse();

            StandardRoutePolicy.DetermineHandlerSource(SessionStateRequirement.DoesNotUseSessionState, chain)
                .ShouldBeOfType<SessionlessSynchronousHttpHandlerSource>();
        }

        [Test]
        public void DetermineHandlerSource_for_synchronous_with_session()
        {
            var chain = BehaviorChain.For<Action2>(x => x.M1());
            chain.IsAsynchronous().ShouldBeFalse();

            StandardRoutePolicy.DetermineHandlerSource(SessionStateRequirement.RequiresSessionState, chain)
                .ShouldBeOfType<SynchronousHttpHandlerSource>();
        }

        [Test]
        public void DetermineHandlerSource_for_asynch_and_sessionless()
        {
            var chain = BehaviorChain.For<Action3>(x => x.M1Async());
            chain.IsAsynchronous().ShouldBeTrue();

            StandardRoutePolicy.DetermineHandlerSource(SessionStateRequirement.DoesNotUseSessionState, chain)
                .ShouldBeOfType<SessionlessAsynchronousHttpHandlerSource>();
        }

        [Test]
        public void DetermineHandlerSource_for_async_and_requires_session()
        {
            var chain = BehaviorChain.For<Action3>(x => x.M1Async());
            chain.IsAsynchronous().ShouldBeTrue();

            StandardRoutePolicy.DetermineHandlerSource(SessionStateRequirement.RequiresSessionState, chain)
                .ShouldBeOfType<AsynchronousHttpHandlerSource>();
        }
        
        [Test]
        public void it_builds_routes_for_all_actions()
        {
            _routes.ShouldHaveCount(_actionIds.Count());
        }

        [Test]
        public void it_assigns_routehandler_on_route()
        {
            _routes.Each(r => r.RouteHandler.ShouldBeOfType<IFubuRouteHandler>());
        }

        private BehaviorGraph setupActions()
        {          
            var registry = new FubuRegistry();
            registry.Route("a/m1").Calls<Action1>(a => a.M1());
            registry.Route("a/m2").Calls<Action1>(a => a.M2());
            registry.Route("b/m1").Calls<Action2>(b => b.M1());
            registry.Route("b/m2").Calls<Action2>(b => b.M2());
            registry.Route("c/m1async").Calls<Action3>(b => b.M1Async());
            return BehaviorGraph.BuildFrom(registry);
        }

        public class ServiceFactory : IServiceFactory
        {
            private readonly IEnumerable<Guid> _behaviorIds;
            public ServiceFactory(IEnumerable<Guid> behaviorIds) { _behaviorIds = behaviorIds; }

            public IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId)
            {
                return _behaviorIds.Contains(behaviorId) ? new ActionBehavior(behaviorId) : null;
            }

            public T Get<T>()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<T> GetAll<T>()
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
        public class ActionBehavior : IActionBehavior
        {
            public ActionBehavior(Guid behaviorId) { BehaviorId = behaviorId; }
            public Guid BehaviorId { get; private set; }
            public void Invoke() {}
            public void InvokePartial() {}
        }
        public class Action1
        {
            public void M1(){}
            public void M2(){}
        }
        public class Action2
        {
            public void M1() { }
            public void M2() { }
        }
        public class Action3
        {
            public Task<object> M1Async()
            {
                return null;
            }
        }
    }
}