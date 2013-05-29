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
        public IList<RouteBase> BuildRoutes(BehaviorGraph graph, IServiceFactory factory)
        {
            var defaultSessionRequirement = graph.Settings.Get<SessionStateRequirement>();
            return graph
                .Behaviors
                .Where(x => x.Route != null)
                .OrderBy(x => x.Route.Rank)
                .SelectMany(x => BuildRoute(factory, defaultSessionRequirement, x))
                .ToList();
        }

        public IEnumerable<RouteBase> BuildRoute(IServiceFactory factory, SessionStateRequirement defaultSessionRequirement, BehaviorChain chain)
        {
            yield return buildRoute(factory, defaultSessionRequirement, chain, chain.Route);

            foreach (var alias in chain.AdditionalRoutes)
            {
                yield return buildRoute(factory, defaultSessionRequirement, chain, alias);
            }
        }

        private RouteBase buildRoute(IServiceFactory factory, SessionStateRequirement defaultSessionRequirement,
                                    BehaviorChain chain, IRouteDefinition routeDefinition)
        {
            var requiresSession = routeDefinition.SessionStateRequirement ?? defaultSessionRequirement;

            var route = routeDefinition.ToRoute();
            var handlerSource = DetermineHandlerSource(requiresSession, chain);
            var invoker = DetermineInvoker(factory, chain);

            route.RouteHandler = new FubuRouteHandler(invoker, handlerSource);

            return route;
        }

        public static IBehaviorInvoker DetermineInvoker(IServiceFactory factory, BehaviorChain chain)
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