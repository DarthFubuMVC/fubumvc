using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Handlers;

namespace FubuMVC.Core.Routing
{
    public class StandardRoutePolicy : IRoutePolicy
    {
        public IList<RouteBase> BuildRoutes(BehaviorGraph graph, IBehaviorFactory factory)
        {
            var defaultSessionRequirement = graph.Settings.Get<SessionStateRequirement>();
            return graph.Behaviors.Where(x => x.Route != null).OrderBy(x => x.Route.Rank).Select(x => BuildRoute(factory, defaultSessionRequirement, x)).ToList();
        }

        public RouteBase BuildRoute(IBehaviorFactory factory, SessionStateRequirement defaultSessionRequirement, BehaviorChain chain)
        {
            var requiresSession = chain.Route.SessionStateRequirement ?? defaultSessionRequirement;

            var route = chain.Route.ToRoute();
            var handlerSource = DetermineHandlerSource(requiresSession, chain);
            var invoker = DetermineInvoker(factory, chain);

            route.RouteHandler = new FubuRouteHandler(invoker, handlerSource);

            return route;
        }

        public static IBehaviorInvoker DetermineInvoker(IBehaviorFactory factory, BehaviorChain chain)
        {
            return chain.IsAsynchronous() ? new AsyncBehaviorInvoker(factory, chain) : new BehaviorInvoker(factory, chain);
        }

        public static IHttpHandlerSource DetermineHandlerSource(SessionStateRequirement sessionStateRequirement, BehaviorChain chain)
        {
            if (chain.IsAsynchronous())
            {
                return sessionStateRequirement == SessionStateRequirement.RequiresSessionState
                           ? (IHttpHandlerSource) new AsynchronousHttpHandlerSource()
                           : new SessionlessAsynchronousHttpHandlerSource();
            }

            return sessionStateRequirement == SessionStateRequirement.RequiresSessionState
                       ? (IHttpHandlerSource)new SynchronousHttpHandlerSource()
                       : new SessionlessSynchronousHttpHandlerSource();
        }
    }
}