using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Conventions
{
    // TODO -- need a way to ignore routes
    public class RouteDefinitionResolver
    {
        private readonly UrlPolicy _defaultUrlPolicy;
        private readonly RouteInputPolicy _inputPolicy = new RouteInputPolicy();
        private readonly List<IUrlPolicy> _policies = new List<IUrlPolicy>();
        private readonly RouteConstraintPolicy _constraintPolicy = new RouteConstraintPolicy();
        private IConfigurationObserver _observer;

        public RouteDefinitionResolver()
        {
            _defaultUrlPolicy = new UrlPolicy(call => true, _inputPolicy);
            _defaultUrlPolicy.IgnoreClassSuffix("controller");

            _inputPolicy.PropertyFilters.Includes +=
                prop => prop.InputProperty.HasAttribute<RouteInputAttribute>();

            _inputPolicy.PropertyFilters.Includes +=
                prop => prop.InputProperty.HasAttribute<QueryStringAttribute>();
            
            _inputPolicy.PropertyAlterations.Register(prop => prop.HasAttribute<QueryStringAttribute>(),
                                                      (route, prop) => route.Input.AddQueryInput(prop));

            _policies.Add(new UrlPatternAttributePolicy());
        }

        public RouteInputPolicy InputPolicy { get { return _inputPolicy; } }
        public UrlPolicy DefaultUrlPolicy { get { return _defaultUrlPolicy; } }
        public RouteConstraintPolicy ConstraintPolicy { get { return _constraintPolicy; } }

        public void Apply(BehaviorGraph graph, BehaviorChain chain)
        {
            // Don't override the route if it already exists
            if (chain.Route != null)
            {
                return;
            }

            // Don't override the route if the chain is a partial
            if (chain.IsPartialOnly)
            {
                return;
            }

            _observer = graph.Observer;

            ActionCall call = chain.Calls.FirstOrDefault();
            if (call == null) return;

            IUrlPolicy policy = _policies.FirstOrDefault(x => x.Matches(call, _observer)) ?? _defaultUrlPolicy;
            _observer.RecordCallStatus(call, "First matching UrlPolicy (or default): {0}".ToFormat(policy.GetType().Name));
            
            IRouteDefinition route = policy.Build(call);
            _constraintPolicy.Apply(call, route, _observer);
            
            _observer.RecordCallStatus(call, "Route definition determined by url policy: [{0}]".ToFormat(route.ToRoute().Url));
            chain.Route = route;
        }

        public void ApplyToAll(BehaviorGraph graph)
        {
            graph.Behaviors.ToList().Each(chain => Apply(graph, chain));
        }

        public void RegisterUrlPolicy(IUrlPolicy policy)
        {
            RegisterUrlPolicy(policy, false);
        }
        
        public void RegisterUrlPolicy(IUrlPolicy policy, bool prepend)
        {
            if (prepend)
            {
                _policies.Insert(0, policy);
            }
            else
            {
                _policies.Add(policy);
            }
        }

        public void RegisterRouteInputPolicy(Func<ActionCall, bool> where, Action<IRouteDefinition, ActionCall> action)
        {
            _inputPolicy.InputBuilders.Register(where, action);
        }
    }
}