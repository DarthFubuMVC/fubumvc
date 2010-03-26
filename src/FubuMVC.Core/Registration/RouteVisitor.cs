using System;
using FubuCore.Util;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration
{
    public class RouteVisitor : IRouteVisitor, IConfigurationAction
    {
        private readonly CompositeAction<IRouteDefinition, BehaviorChain> _actions =
            new CompositeAction<IRouteDefinition, BehaviorChain>();

        private readonly CompositePredicate<BehaviorChain> _behaviorFilters = new CompositePredicate<BehaviorChain>();
        private readonly CompositePredicate<IRouteDefinition> _routeFilters = new CompositePredicate<IRouteDefinition>();

        public CompositeAction<IRouteDefinition, BehaviorChain> Actions { get { return _actions; } set { } }
        public CompositePredicate<BehaviorChain> BehaviorFilters { get { return _behaviorFilters; } set { } }
        public CompositePredicate<IRouteDefinition> RouteFilters { get { return _routeFilters; } set { } }

        void IConfigurationAction.Configure(BehaviorGraph graph)
        {
            throw new NotImplementedException();
        }

        public void VisitRoute(IRouteDefinition route, BehaviorChain chain)
        {
            if (_behaviorFilters.MatchesAll(chain) && _routeFilters.MatchesAll(route))
            {
                _actions.Do(route, chain);
            }
        }
    }
}