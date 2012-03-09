using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Http;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI;
using FubuMVC.Core.View.Activation;
using FubuMVC.Core.View.Attachment;
using System.Linq;

namespace FubuMVC.Core
{
    /// <summary>
    /// The <see cref="FubuRegistry"/> class provides methods and grammars for configuring FubuMVC.
    /// Using a <see cref="FubuRegistry"/> subclass is the recommended way of configuring FubuMVC.
    /// </summary>
    /// <example>
    /// public class MyFubuRegistry : FubuRegistry
    /// {
    ///     public MyFubuRegistry()
    ///     {
    ///         Applies.ToThisAssembly();
    ///     }
    /// }
    /// </example>
    public partial class FubuRegistry
    {
        private readonly List<IConfigurationAction> _conventions = new List<IConfigurationAction>();
        private readonly List<IConfigurationAction> _explicits = new List<IConfigurationAction>();
        private readonly List<RegistryImport> _imports = new List<RegistryImport>();
        private readonly IList<Action<FubuRegistry>> _importsConventions = new List<Action<FubuRegistry>>();
        private readonly List<IConfigurationAction> _policies = new List<IConfigurationAction>();
        private readonly RouteDefinitionResolver _routeResolver = new RouteDefinitionResolver();
        private readonly IList<IServiceRegistry> _serviceRegistrations = new List<IServiceRegistry>();
        private readonly List<IConfigurationAction> _systemPolicies = new List<IConfigurationAction>();
        private readonly TypePool _types = new TypePool(FindTheCallingAssembly());
        private readonly ViewAttacherConvention _viewAttacherConvention;
        private readonly ViewBagConventionRunner _bagRunner;
        private IConfigurationObserver _observer;
        
        private readonly ConnegAttachmentPolicy _connegAttachmentPolicy;
        private readonly BehaviorAggregator _behaviorAggregator;
        private readonly List<IActionSource> _actionSources = new List<IActionSource>();

        public FubuRegistry()
        {
            _behaviorAggregator = new BehaviorAggregator(_types, _actionSources);
            _observer = new NulloConfigurationObserver();
            _viewAttacherConvention = new ViewAttacherConvention();
            _bagRunner = new ViewBagConventionRunner(_types);
            _connegAttachmentPolicy = new ConnegAttachmentPolicy(_types);

            setupDefaultConventionsAndPolicies();
        }


        public FubuRegistry(Action<FubuRegistry> configure) : this()
        {
            configure(this);
        }

        /// <summary>
        /// Gets the name of the <see cref="FubuRegistry"/>. Mostly used for diagnostics.
        /// </summary>
        public virtual string Name
        {
            get { return GetType().ToString(); }
        }

        /// <summary>
        /// Finds the currently executing assembly.
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


        private IEnumerable<IServiceRegistry> allServiceRegistrations()
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

        /// <summary>
        /// Constructs a <see cref="BehaviorGraph"/> using the configuration expressions defined in this <see cref="FubuRegistry"/>. This method is mostly for internal usage.
        /// </summary>
        /// <returns></returns>
        public BehaviorGraph BuildGraph()
        {
            Import<AssetServicesRegistry>();

            Services<ModelBindingServicesRegistry>();
            Services<SecurityServicesRegistry>();
            Services<ResourcesServiceRegistry>();
            Services<HtmlConventionServiceRegistry>();
            Services<PackagingServiceRegistry>();
            Services<HttpStandInServiceRegistry>();
            Services<ViewActivationServiceRegistry>();
            Services<CoreServiceRegistry>();

            return BuildLightGraph();
        }

        private readonly IList<Action<TypePool>> _scanningOperations = new List<Action<TypePool>>();

        /// <summary>
        /// Access the TypePool with all the assemblies represented in the AppliesTo expressions
        /// to make conventional registrations of any kind
        /// </summary>
        /// <param name="configuration"></param>
        public void WithTypes(Action<TypePool> configuration)
        {
            _scanningOperations.Add(configuration);
        }

        public BehaviorGraph BuildLightGraph()
        {
            var graph = new BehaviorGraph(_observer);

            _scanningOperations.Each(x => x(_types));

            // Service registrations from imports
            allServiceRegistrations().OfType<IConfigurationAction>().Each(x => x.Configure(graph));

            _conventions.Configure(graph);

            // Importing behavior chains from imports
            var observerImporter = new ObserverImporter(graph.Observer);
            _imports.Each(x =>
            {
                _importsConventions.Each(c => c(x.Registry));
                x.ImportInto(graph);
                observerImporter.Import(x.Registry._observer);
            });

            _explicits.Configure(graph);

            _policies.Configure(graph);
            _systemPolicies.Configure(graph);

            

            return graph;
        }
    }
}