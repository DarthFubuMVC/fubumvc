using System;
using System.Collections.Generic;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuMVC.Core.View.WebForms;

namespace FubuMVC.Core
{
    // Register more and different types of actions
    public interface IActionSource
    {
        IEnumerable<ActionCall> FindActions(TypePool types);
    }

    public partial class FubuRegistry
    {
        private readonly ActionSourceMatcher _actionSourceMatcher = new ActionSourceMatcher();
        private readonly BehaviorMatcher _behaviorMatcher = new BehaviorMatcher();

        private readonly List<IConfigurationAction> _conventions = new List<IConfigurationAction>();
        private readonly List<IConfigurationAction> _explicits = new List<IConfigurationAction>();
        private readonly List<RegistryImport> _imports = new List<RegistryImport>();
        private readonly IPartialViewTypeRegistry _partialViewTypes = new PartialViewTypeRegistry();
        private readonly List<IConfigurationAction> _policies = new List<IConfigurationAction>();
        private readonly RouteDefinitionResolver _routeResolver = new RouteDefinitionResolver();
        private readonly List<IConfigurationAction> _systemPolicies = new List<IConfigurationAction>();
        private readonly TypePool _types = new TypePool();
        private readonly IList<Action<IServiceRegistry>> _serviceRegistrations = new List<Action<IServiceRegistry>>();
        private readonly ViewAttacher _viewAttacher;
        private IConfigurationObserver _observer;


        public FubuRegistry()
        {
            _observer = new NulloConfigurationObserver();
            _viewAttacher = new ViewAttacher(_types);

            setupDefaultConventionsAndPolicies();
        }


        public FubuRegistry(Action<FubuRegistry> configure)
            : this()
        {
            configure(this);
        }

        private void addConvention(Action<BehaviorGraph> action)
        {
            var convention = new LambdaConfigurationAction(action);
            _conventions.Add(convention);
        }

        private void addExplicit(Action<BehaviorGraph> action)
        {
            var convention = new LambdaConfigurationAction(action);
            _explicits.Add(convention);
        }

        public BehaviorGraph BuildGraph()
        {
            var graph = new BehaviorGraph(_observer);


            // TODO -- will need to get reordered to pull service registrations from imports
            _serviceRegistrations.Each(x => x(graph.Services));
            setupServices(graph);

            _conventions.Configure(graph);

            // imports will need to go above service registrations
            _imports.Each(x => x.ImportInto(graph));
            _explicits.Configure(graph);

            _policies.Configure(graph);
            _systemPolicies.Configure(graph);

            return graph;
        }
    }

    public interface IFubuRegistryExtension
    {
        void Configure(FubuRegistry registry);
    }
}