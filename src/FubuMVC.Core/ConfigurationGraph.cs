using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Services;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Resources.PathBased;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI.Navigation;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core
{
    /// <summary>
    ///   Orders and governs the construction of a BehaviorGraph
    /// </summary>
    public class ConfigurationGraph
    {
        private readonly List<IActionSource> _actionSources = new List<IActionSource>();

        private readonly Cache<ConfigurationType, IList<IConfigurationAction>> _configurations
            = new Cache<ConfigurationType, IList<IConfigurationAction>>(x => new List<IConfigurationAction>());

        private readonly List<RegistryImport> _imports = new List<RegistryImport>();
        private readonly FubuRegistry _registry;
        private readonly RouteDefinitionResolver _routeResolver = new RouteDefinitionResolver();
        private readonly TypePool _types = new TypePool(FindTheCallingAssembly());

        public ConfigurationGraph(FubuRegistry registry)
        {
            _registry = registry;
        }

        public TypePool Types
        {
            get { return _types; }
        }

        public RouteDefinitionResolver RouteResolver
        {
            get { return _routeResolver; }
        }

        public void AddConfiguration(IConfigurationAction action, ConfigurationType? defaultType = null)
        {
            ConfigurationType? type = DetermineConfigurationType(action) ?? defaultType;
            if (type == null)
            {
                throw new ArgumentOutOfRangeException(
                    "No ConfigurationType specified and unable to determine what the configuration type for " +
                    action.GetType());
            }

            _configurations[type.Value].FillAction(action);
        }

        public BehaviorGraph Build()
        {
            var graph = new BehaviorGraph();

            graph.Types = Types;

            AllServiceRegistrations().Each(services => {
                graph.Log.RunAction(_registry, services);

                // TODO -- temporary.  I think this will need to get better so that you can trace the source all the way through
                graph.Log.EventsOfType<ServiceEvent>().Where(x => x.RegistrationSource == null).Each(
                    x => x.RegistrationSource = services.GetType().Name);
            });

            AllConfigurationActions().Each(x => { graph.Log.RunAction(_registry, x); });

            graph.Services.AddService(this);

            return graph;
        }

        public BehaviorGraph BuildForImport(BehaviorGraph parent)
        {
            IEnumerable<IConfigurationAction> lightweightActions = AllDiscoveryActions()
                .Union(_imports)
                .Union(_configurations[ConfigurationType.Explicit])
                .Union(_configurations[ConfigurationType.Policy])
                .Union(new IConfigurationAction[]{new ViewAttacher()})
                .Union(_configurations[ConfigurationType.Reordering]);

            BehaviorGraph graph = BehaviorGraph.ForChild(parent);

            lightweightActions.Each(x => graph.Log.RunAction(_registry, x));

            return graph;
        }

        private IEnumerable<IConfigurationAction> allChildrenImports()
        {
            foreach (RegistryImport import in _imports)
            {
                foreach (RegistryImport action in import.Registry.Configuration._imports)
                {
                    yield return action;

                    foreach (
                        IConfigurationAction descendentAction in
                            _imports.SelectMany(x => x.Registry.Configuration.allChildrenImports()))
                    {
                        yield return descendentAction;
                    }
                }
            }
        }

        public IEnumerable<IConfigurationAction> UniqueImports()
        {
            List<IConfigurationAction> children = allChildrenImports().ToList();

            return _imports.Where(x => !children.Contains(x));
        }

        public IEnumerable<IConfigurationAction> AllConfigurationActions()
        {
            return AllDiscoveryActions()
                .Union(UniqueImports())
                .Union(_configurations[ConfigurationType.Explicit])
                .Union(AllPolicies())
                .Union(SystemPolicies())
                .Union(_configurations[ConfigurationType.ByNavigation])
                .Union(_configurations[ConfigurationType.Reordering])
                .Union(_configurations[ConfigurationType.Instrumentation]);
        }

        public IEnumerable<IConfigurationAction> ConfigurationsByType(ConfigurationType configurationType)
        {
            return _configurations[configurationType];
        }

        public IEnumerable<IConfigurationAction> SystemPolicies()
        {
            return fullGraphPolicies()
                .Union(navigationRegistrations().OfType<IConfigurationAction>())
                .Union(new IConfigurationAction[] {new MenuItemAttributeConfigurator(), new CompileNavigationStep()});
        }

        public IList<IConfigurationAction> AllPolicies()
        {
            return _configurations[ConfigurationType.Policy];
        }

        public IEnumerable<IConfigurationAction> AllServiceRegistrations()
        {
            return serviceRegistrations()
                .Union(systemServices());
        }

        private static IEnumerable<IConfigurationAction> fullGraphPolicies()
        {
            yield return new ViewAttacher();
            yield return new ActionlessViewConvention();

            yield return new AssetContentEndpoint();

            yield return new UrlPatternAttributeOnViewModelPolicy();
            yield return new ModifyChainAttributeConvention();
            yield return new ResourcePathRoutePolicy();
            yield return new MissingRouteInputPolicy();

            yield return new ContinuationHandlerConvention();
            yield return new AsyncContinueWithHandlerConvention();

            yield return new CachedPartialConvention();

            yield return new JsonMessageInputConvention();
            yield return new AjaxContinuationPolicy();
            yield return new DictionaryOutputConvention();
            yield return new StringOutputPolicy();
            yield return new HtmlTagOutputPolicy();

            yield return new DefaultOutputPolicy();

            yield return new CacheAttributePolicy();

            yield return new AttachAuthorizationPolicy();
            yield return new AttachInputPolicy();
            yield return new AttachOutputPolicy();

            yield return new OutputBeforeAjaxContinuationPolicy();

            yield return new ReorderBehaviorsPolicy
            {
                WhatMustBeBefore = node => node.Category == BehaviorCategory.Authentication,
                WhatMustBeAfter = node => node.Category == BehaviorCategory.Authorization
            };

            yield return new ReorderBehaviorsPolicy
            {
                WhatMustBeBefore = node => node is OutputCachingNode,
                WhatMustBeAfter = node => node is OutputNode
            };
        }

        public IEnumerable<IConfigurationAction> AllDiscoveryActions()
        {
            if (_actionSources.Any())
            {
                yield return new BehaviorAggregator(_types, _actionSources);
            }
            else
            {
                yield return new BehaviorAggregator(_types, new IActionSource[] {new EndpointActionSource()});
            }

            yield return new PartialOnlyConvention();
            yield return _routeResolver;

            foreach (IConfigurationAction action in _configurations[ConfigurationType.Discovery])
            {
                yield return action;
            }
        }

        private static IEnumerable<ServiceRegistry> systemServices()
        {
            yield return new AssetServicesRegistry();
            yield return new ModelBindingServicesRegistry();
            yield return new SecurityServicesRegistry();
            yield return new HttpStandInServiceRegistry();
            yield return new CoreServiceRegistry();
            yield return new NavigationServiceRegistry();
            yield return new CachingServiceRegistry();
        }

        private IEnumerable<IConfigurationAction> serviceRegistrations()
        {
            foreach (RegistryImport import in _imports)
            {
                foreach (IConfigurationAction action in import.Registry.Configuration.serviceRegistrations())
                {
                    yield return action;
                }
            }

            foreach (IConfigurationAction action in _configurations[ConfigurationType.Services])
            {
                yield return action;
            }

            yield return new FileRegistration();
        }

        private IEnumerable<IConfigurationAction> navigationRegistrations()
        {
            foreach (IConfigurationAction action in _configurations[ConfigurationType.Navigation])
            {
                yield return action;
            }

            foreach (RegistryImport import in _imports)
            {
                foreach (IConfigurationAction action in import.Registry.Configuration.navigationRegistrations())
                {
                    yield return action;
                }
            }
        }

        public void AddImport(RegistryImport import)
        {
            if (HasImported(import.Registry)) return;

            _imports.Add(import);
        }

        public bool HasImported(FubuRegistry registry)
        {
            if (_imports.Any(x => x.Registry.GetType() == registry.GetType()))
            {
                return true;
            }

            if (_imports.Any(x => x.Registry.Configuration.HasImported(registry)))
            {
                return true;
            }

            return false;
        }

        public void AddActions(IActionSource source)
        {
            _actionSources.FillAction(source);
        }

        /// <summary>
        ///   Finds the currently executing assembly.
        /// </summary>
        /// <returns></returns>
        public static Assembly FindTheCallingAssembly()
        {
            var trace = new StackTrace(false);

            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            Assembly fubuCore = typeof (ITypeResolver).Assembly;

            Assembly callingAssembly = null;
            for (int i = 0; i < trace.FrameCount; i++)
            {
                StackFrame frame = trace.GetFrame(i);
                Assembly assembly = frame.GetMethod().DeclaringType.Assembly;
                if (assembly != thisAssembly && assembly != fubuCore)
                {
                    callingAssembly = assembly;
                    break;
                }
            }
            return callingAssembly;
        }

        public static ConfigurationType? DetermineConfigurationType(IConfigurationAction action)
        {
            if (action is ReorderBehaviorsPolicy) return ConfigurationType.Reordering;
            if (action is NavigationRegistry) return ConfigurationType.Navigation;
            if (action is ServiceRegistry) return ConfigurationType.Services;

            if (action.GetType().HasAttribute<ConfigurationTypeAttribute>())
            {
                return action.GetType().GetAttribute<ConfigurationTypeAttribute>().ConfigurationType;
            }

            return null;
        }

        #region Nested type: FileRegistration

        internal class FileRegistration : IConfigurationAction
        {
            #region IConfigurationAction Members

            public void Configure(BehaviorGraph graph)
            {
                graph.Services.Clear(typeof (IFubuApplicationFiles));
                graph.Services.AddService(graph.Files);
            }

            #endregion
        }

        #endregion
    }
}