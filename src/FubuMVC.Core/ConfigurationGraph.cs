using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Resources.PathBased;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Navigation;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Activation;
using FubuMVC.Core.View.Attachment;
using System.Linq;

namespace FubuMVC.Core
{
    /// <summary>
    ///   Orders and governs the construction of a BehaviorGraph
    /// </summary>
    public class ConfigurationGraph
    {
        private readonly RouteDefinitionResolver _routeResolver = new RouteDefinitionResolver();
        private readonly List<IActionSource> _actionSources = new List<IActionSource>();
        private readonly IList<IConfigurationAction> _conventions = new List<IConfigurationAction>();
        private readonly IList<IConfigurationAction> _explicits = new List<IConfigurationAction>();
        private readonly IList<IConfigurationAction> _policies = new List<IConfigurationAction>();
        private readonly IList<IServiceRegistry> _serviceRegistrations = new List<IServiceRegistry>();
        private readonly IList<ReorderBehaviorsPolicy> _reorderRules = new List<ReorderBehaviorsPolicy>();
        private readonly List<RegistryImport> _imports = new List<RegistryImport>();
        private readonly TypePool _types = new TypePool(FindTheCallingAssembly());
        private readonly IViewEngineRegistry _engineRegistry = new ViewEngineRegistry();
        private readonly ViewAttacher _views = new ViewAttacher();

        public void Build(BehaviorGraph graph)
        {
            graph.Views = _engineRegistry.BuildViewBag();

            allActions().Each(x => x.Configure(graph));
        }

        public BehaviorGraph BuildForImport(ViewBag views)
        {
            var lightweightActions = allConventions()
                .Union(_imports)
                .Union(_explicits)
                .Union(_policies)
                .Union(new IConfigurationAction[]{_views})
                .Union(_reorderRules);

            var graph = new BehaviorGraph{
                Views = views
            };

            lightweightActions.Each(x => x.Configure(graph));

            return graph;
        }

        public ViewAttacher Views
        {
            get { return _views; }
        }


        private IEnumerable<IConfigurationAction> allActions()
        {
            return serviceRegistrations().OfType<IConfigurationAction>()
                .Union(systemServices())
                .Union(allConventions())
                .Union(_imports)
                .Union(_explicits)
                .Union(_policies)
                .Union(new IConfigurationAction[] { _views })
                .Union(fullGraphPolicies())
                .Union(_reorderRules);
        }

        private static IEnumerable<IConfigurationAction> fullGraphPolicies()
        {
            yield return new ModifyChainAttributeConvention();
            yield return new ResourcePathRoutePolicy();
            yield return new MissingRouteInputPolicy();

            yield return new ContinuationHandlerConvention();
            yield return new AsyncContinueWithHandlerConvention();

            yield return new HeaderWritingPolicy();
            yield return new JsonMessageInputConvention();
            yield return new AjaxContinuationPolicy();
            yield return new DictionaryOutputConvention();
            yield return new StringOutputPolicy();
            yield return new HtmlTagOutputPolicy();

            yield return new AttachAuthorizationPolicy();
            yield return new AttachInputPolicy();
            yield return new AttachOutputPolicy();


        }

        private IEnumerable<IConfigurationAction> allConventions()
        {
            yield return new BehaviorAggregator(_types, _actionSources);
            yield return new PartialOnlyConvention();
            yield return _routeResolver;

            foreach (var action in _conventions)
            {
                yield return action;
            }
        }

        private static IEnumerable<IConfigurationAction> systemServices()
        {
            yield return new ModelBindingServicesRegistry();
            yield return new SecurityServicesRegistry();
            yield return new HtmlConventionServiceRegistry();
            yield return new PackagingServiceRegistry();
            yield return new HttpStandInServiceRegistry();
            yield return new ViewActivationServiceRegistry();
            yield return new CoreServiceRegistry();
            yield return new NavigationServiceRegistry();
        }

        private IEnumerable<IServiceRegistry> serviceRegistrations()
        {
            foreach (var import in _imports)
            {
                foreach (var action in import.Registry.Configuration.serviceRegistrations())
                {
                    yield return action;
                }
            }

            foreach (var action in _serviceRegistrations)
            {
                yield return action;
            }
        }

        public TypePool Types
        {
            get { return _types; }
        }

        public RouteDefinitionResolver RouteResolver
        {
            get { return _routeResolver; }
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

        public void AddServices(IServiceRegistry registry)
        {
            _serviceRegistrations.Add(registry);
        }

        public void AddActions(IActionSource source)
        {
            _actionSources.FillAction(source);
        }

        public void AddConvention(IConfigurationAction convention)
        {
            _conventions.FillAction(convention);
        }


        public void AddPolicy(IConfigurationAction policy)
        {
            if (policy is ReorderBehaviorsPolicy)
            {
                _reorderRules.Add((ReorderBehaviorsPolicy) policy);
            }
            else
            {
                _policies.FillAction(policy);
            }
        }

        public void AddExplicit(IConfigurationAction explicitAction)
        {
            _explicits.Add(explicitAction);
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