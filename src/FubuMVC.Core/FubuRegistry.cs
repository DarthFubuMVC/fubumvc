using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;

namespace FubuMVC.Core
{
    public partial class FubuRegistry
    {
        private readonly ActionSourceMatcher _actionSourceMatcher = new ActionSourceMatcher();
        private readonly BehaviorMatcher _behaviorMatcher = new BehaviorMatcher();

        private readonly List<IConfigurationAction> _conventions = new List<IConfigurationAction>();
        private readonly List<IConfigurationAction> _explicits = new List<IConfigurationAction>();
        private readonly List<RegistryImport> _imports = new List<RegistryImport>();
        private readonly List<IConfigurationAction> _policies = new List<IConfigurationAction>();
        private readonly RouteDefinitionResolver _routeResolver = new RouteDefinitionResolver();
        private readonly IList<Action<IServiceRegistry>> _serviceRegistrations = new List<Action<IServiceRegistry>>();
        private readonly List<IConfigurationAction> _systemPolicies = new List<IConfigurationAction>();
        private readonly TypePool _types = new TypePool(FindTheCallingAssembly());
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

        public static Assembly FindTheCallingAssembly()
        {
            var trace = new StackTrace(false);

            var thisAssembly = Assembly.GetExecutingAssembly();
            var fubuCore = typeof (ITypeResolver).Assembly;

            Assembly callingAssembly = null;
            for (var i = 0; i < trace.FrameCount; i++)
            {
                var frame = trace.GetFrame(i);
                var assembly = frame.GetMethod().DeclaringType.Assembly;
                if (assembly != thisAssembly && assembly != fubuCore)
                {
                    callingAssembly = assembly;
                    break;
                }
            }
            return callingAssembly;
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


        private IEnumerable<Action<IServiceRegistry>> allServiceRegistrations()
        {
            foreach (var import in _imports)
            {
                foreach (var action in import.Registry.allServiceRegistrations())
                {
                    yield return action;
                }
            }

            foreach (var action in _serviceRegistrations)
            {
                yield return action;
            }
        }


        public BehaviorGraph BuildGraph()
        {
            var graph = new BehaviorGraph(_observer);

            // Service registrations from imports
            allServiceRegistrations().Each(x => x(graph.Services));

            setupServices(graph);

            _conventions.Configure(graph);

            // Importing behavior chains from imports
            _imports.Each(x => x.ImportInto(graph));

            _explicits.Configure(graph);

            _policies.Configure(graph);
            _systemPolicies.Configure(graph);

            return graph;
        }
    }
}