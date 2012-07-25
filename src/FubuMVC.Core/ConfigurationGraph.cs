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
using FubuMVC.Core.Http;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Services;
using FubuMVC.Core.Resources.PathBased;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Navigation;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Activation;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationTypeAttribute : Attribute
    {
        private readonly ConfigurationType _configurationType;

        public ConfigurationTypeAttribute(ConfigurationType configurationType)
        {
            _configurationType = configurationType;
        }

        public ConfigurationType ConfigurationType
        {
            get { return _configurationType; }
        }
    }

    public class PolicyAttribute : ConfigurationTypeAttribute
    {
        public PolicyAttribute()
            : base(ConfigurationType.Policy)
        {
        }
    }

    public class DiscoveryAttribute : ConfigurationTypeAttribute
    {
        public DiscoveryAttribute()
            : base(ConfigurationType.Discovery)
        {
        }
    }

    public enum ConfigurationType
    {
        Discovery,
        Explicit,
        Policy,
        Reordering,
        Navigation,
        Instrumentation,
        Services
    }


    /// <summary>
    ///   Orders and governs the construction of a BehaviorGraph
    /// </summary>
    public class ConfigurationGraph
    {
        private readonly List<IActionSource> _actionSources = new List<IActionSource>();

        private readonly Cache<ConfigurationType, IList<IConfigurationAction>> _configurations
            = new Cache<ConfigurationType, IList<IConfigurationAction>>(x => new List<IConfigurationAction>());

        private readonly IViewEngineRegistry _engineRegistry = new ViewEngineRegistry();
        private readonly List<RegistryImport> _imports = new List<RegistryImport>();
        private readonly RouteDefinitionResolver _routeResolver = new RouteDefinitionResolver();
        private readonly TypePool _types = new TypePool(FindTheCallingAssembly());
        private readonly ViewAttacher _views = new ViewAttacher();

        public ViewAttacher Views
        {
            get { return _views; }
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
            var type = DetermineConfigurationType(action) ?? defaultType;
            if (type == null)
            {
                throw new ArgumentOutOfRangeException(
                    "No ConfigurationType specified and unable to determine what the configuration type for " +
                    action.GetType());
            }

            _configurations[type.Value].FillAction(action);
        }

        public void Build(BehaviorGraph graph)
        {
            graph.Views = _engineRegistry.BuildViewBag(_types);

            allActions().Each(x => x.Configure(graph));
        }

        public BehaviorGraph BuildForImport(ViewBag views)
        {
            var lightweightActions = allConventions()
                .Union(_imports)
                .Union(_configurations[ConfigurationType.Explicit])
                .Union(_configurations[ConfigurationType.Policy])
                .Union(new IConfigurationAction[]{_views})
                .Union(_configurations[ConfigurationType.Reordering]);

            var graph = new BehaviorGraph{
                Views = views
            };

            lightweightActions.Each(x => x.Configure(graph));

            return graph;
        }


        private IEnumerable<IConfigurationAction> allActions()
        {
            return serviceRegistrations().OfType<IConfigurationAction>()
                .Union(systemServices())
                .Union(serviceRegistrations())
                .Union(allConventions())
                .Union(_imports)
                .Union(_configurations[ConfigurationType.Explicit])
                .Union(_configurations[ConfigurationType.Policy])
                .Union(new IConfigurationAction[]{_views})
                .Union(fullGraphPolicies())
                .Union(navigationRegistrations().OfType<IConfigurationAction>())
                .Union(new IConfigurationAction[]{new MenuItemAttributeConfigurator(), new CompileNavigationStep()})
                .Union(_configurations[ConfigurationType.Reordering])
                .Union(_configurations[ConfigurationType.Instrumentation]);
        }

        private static IEnumerable<IConfigurationAction> fullGraphPolicies()
        {
            yield return new AssetContentEndpoint();

            yield return new ModifyChainAttributeConvention();
            yield return new ResourcePathRoutePolicy();
            yield return new MissingRouteInputPolicy();

            yield return new ContinuationHandlerConvention();
            yield return new AsyncContinueWithHandlerConvention();

            yield return new JsonMessageInputConvention();
            yield return new AjaxContinuationPolicy();
            yield return new DictionaryOutputConvention();
            yield return new StringOutputPolicy();
            yield return new HtmlTagOutputPolicy();

            yield return new DefaultOutputPolicy();

            yield return new AttachAuthorizationPolicy();
            yield return new AttachInputPolicy();
            yield return new AttachOutputPolicy();


            yield return new ReorderBehaviorsPolicy{
                WhatMustBeBefore = node => node.Category == BehaviorCategory.Authentication,
                WhatMustBeAfter = node => node.Category == BehaviorCategory.Authorization
            };
        }

        private IEnumerable<IConfigurationAction> allConventions()
        {
            if (_actionSources.Any())
            {
                yield return new BehaviorAggregator(_types, _actionSources);
            }
            else
            {
                yield return new BehaviorAggregator(_types, new IActionSource[]{new EndpointActionSource()});
            }

            yield return new PartialOnlyConvention();
            yield return _routeResolver;

            foreach (var action in _configurations[ConfigurationType.Discovery])
            {
                yield return action;
            }
        }

        private static IEnumerable<IConfigurationAction> systemServices()
        {
            yield return new AssetServicesRegistry();
            yield return new ModelBindingServicesRegistry();
            yield return new SecurityServicesRegistry();
            yield return new HtmlConventionServiceRegistry();
            yield return new PackagingServiceRegistry();
            yield return new HttpStandInServiceRegistry();
            yield return new ViewActivationServiceRegistry();
            yield return new CoreServiceRegistry();
            yield return new NavigationServiceRegistry();
        }

        private IEnumerable<IConfigurationAction> serviceRegistrations()
        {
            foreach (var import in _imports)
            {
                foreach (var action in import.Registry.Configuration.serviceRegistrations())
                {
                    yield return action;
                }
            }

            foreach (var action in _configurations[ConfigurationType.Services])
            {
                yield return action;
            }
        }

        private IEnumerable<IConfigurationAction> navigationRegistrations()
        {
            foreach (var action in _configurations[ConfigurationType.Navigation])
            {
                yield return action;
            }

            foreach (var import in _imports)
            {
                foreach (var action in import.Registry.Configuration.navigationRegistrations())
                {
                    yield return action;
                }
            }   
        }

        public void AddFacility(IViewFacility facility)
        {
            _engineRegistry.AddFacility(facility);
        }

        public void AddImport(RegistryImport import)
        {
            if (HasImported(import.Registry)) return;

            _imports.Add(import);
        }

        // TODO -- I think this will solve an issue that's been outstanding for a *long* time
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
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CanBeMultiplesAttribute : Attribute
    {
    }

    public static class ConfigurationActionListExtensions
    {
        public static void FillAction<T>(this IList<T> actions, T action)
        {
            var actionType = action.GetType();


            if (TypeIsUnique(actionType) && actions.Any(x => x.GetType() == actionType))
            {
                return;
            }

            actions.Fill(action);
        }

        public static bool TypeIsUnique(Type type)
        {
            if (type.HasAttribute<CanBeMultiplesAttribute>()) return false;

            // If it does not have any non-default constructors
            if (type.GetConstructors().Any(x => x.GetParameters().Any()))
            {
                return false;
            }

            if (type.GetProperties().Any(x => x.CanWrite))
            {
                return false;
            }

            return true;
        }
    }
}