using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Conventions
{
    [Policy]
    [Description("Determines the route for any BehaviorChain that does not already have a Route and is not marked as IsPartialOnly")]
    public class RouteDetermination : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            Configure(graph, graph.Settings.Get<RouteDefinitionResolver>());
        }

        public static void Configure(BehaviorGraph graph, RouteDefinitionResolver resolver)
        {
            //if no default route is specified, this one comes for free
            if (!resolver.HasDefaultRoute())
            {
                resolver.RegisterUrlPolicy(new DefaultRouteConventionBasedUrlPolicy(), true);
            }

            resolver.ApplyToAll(graph);
        }
    }

    // TODO -- need a way to ignore routes
    public class RouteDefinitionResolver
    {
        private readonly RouteConstraintPolicy _constraintPolicy = new RouteConstraintPolicy();
        private readonly UrlPolicy _defaultUrlPolicy;
        private readonly RouteInputPolicy _inputPolicy = new RouteInputPolicy();
        private readonly List<IUrlPolicy> _policies = new List<IUrlPolicy>();

        public RouteDefinitionResolver()
        {
            _defaultUrlPolicy = new UrlPolicy(call => true, _inputPolicy);
            _defaultUrlPolicy.IgnoreClassSuffix("controller");
            _defaultUrlPolicy.IgnoreClassSuffix("endpoint");
            _defaultUrlPolicy.IgnoreClassSuffix("endpoints");

            _inputPolicy.PropertyFilters.Includes +=
                prop => prop.InputProperty.HasAttribute<RouteInputAttribute>();

            _inputPolicy.PropertyFilters.Includes +=
                prop => prop.InputProperty.HasAttribute<QueryStringAttribute>();

            _inputPolicy.PropertyAlterations.Register(prop => prop.HasAttribute<QueryStringAttribute>(),
                                                      (route, prop) => route.Input.AddQueryInput(prop));

            _policies.Add(new UrlPatternAttributePolicy());
        }

        public RouteInputPolicy InputPolicy
        {
            get { return _inputPolicy; }
        }

        public UrlPolicy DefaultUrlPolicy
        {
            get { return _defaultUrlPolicy; }
        }

        public RouteConstraintPolicy ConstraintPolicy
        {
            get { return _constraintPolicy; }
        }

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

            var call = chain.Calls.FirstOrDefault();
            if (call == null) return;

            var policy = _policies.FirstOrDefault(x => x.Matches(call)) ?? _defaultUrlPolicy;

            var route = policy.Build(call);
            _constraintPolicy.Apply(call, route);

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

        public bool HasDefaultRoute()
        {
            return _policies.Any(x => x is DefaultRouteMethodBasedUrlPolicy || x is DefaultRouteInputTypeBasedUrlPolicy);
        }
    }
}