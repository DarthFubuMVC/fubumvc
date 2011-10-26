using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Routing;
using FubuMVC.Core.Runtime;
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
        private IBehaviorFactory theFactory;

        [SetUp]
        public void SetUp()
        {
            theFactory = MockRepository.GenerateMock<IBehaviorFactory>();

            var graph = setupActions();
            _actionIds = graph.Actions().Select(x => x.ParentChain().UniqueId);
            _routes = new StandardRoutePolicy().BuildRoutes(graph, theFactory).Cast<Route>();
        }
        
        [Test]
        public void it_builds_routes_for_all_actions()
        {
            _routes.ShouldHaveCount(_actionIds.Count());
        }

        [Test]
        public void it_assigns_routehandler_on_route()
        {
            _routes.Each(r => r.RouteHandler.ShouldBeOfType<FubuRouteHandler>());
        }

        private BehaviorGraph setupActions()
        {          
            var registry = new FubuRegistry();
            registry.Route("a/m1").Calls<Action1>(a => a.M1());
            registry.Route("a/m2").Calls<Action1>(a => a.M2());
            registry.Route("b/m1").Calls<Action2>(b => b.M1());
            registry.Route("b/m2").Calls<Action2>(b => b.M2());
            return registry.BuildGraph();
        }

        public class BehaviorFactory : IBehaviorFactory
        {
            private readonly IEnumerable<Guid> _behaviorIds;
            public BehaviorFactory(IEnumerable<Guid> behaviorIds) { _behaviorIds = behaviorIds; }

            public IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId)
            {
                return _behaviorIds.Contains(behaviorId) ? new ActionBehavior(behaviorId) : null;
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
    }
}