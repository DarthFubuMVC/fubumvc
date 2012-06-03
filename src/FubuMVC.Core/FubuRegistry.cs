using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core
{
    /// <summary>
    ///   The <see cref = "FubuRegistry" /> class provides methods and grammars for configuring FubuMVC.
    ///   Using a <see cref = "FubuRegistry" /> subclass is the recommended way of configuring FubuMVC.
    /// </summary>
    /// <example>
    ///   public class MyFubuRegistry : FubuRegistry
    ///   {
    ///   public MyFubuRegistry()
    ///   {
    ///   Applies.ToThisAssembly();
    ///   }
    ///   }
    /// </example>
    public partial class FubuRegistry
    {
        private readonly ConfigurationGraph _configuration = new ConfigurationGraph();
        private readonly IList<Type> _importedTypes = new List<Type>();
        private readonly ActionMethodFilter _methodFilter = new ActionMethodFilter();
        private readonly IList<Action<TypePool>> _scanningOperations = new List<Action<TypePool>>();
        private DiagnosticLevel _diagnosticLevel = DiagnosticLevel.None;
        private bool _hasCompiled;

        public FubuRegistry()
        {
        }

        public FubuRegistry(Action<FubuRegistry> configure)
        {
            configure(this);
        }

        public ConfigurationGraph Configuration
        {
            get { return _configuration; }
        }

        /// <summary>
        ///   Modify or configure the settings, data, and policies of the Assets Pipeline
        /// </summary>
        public AssetsExpression Assets
        {
            get { return new AssetsExpression(this); }
        }

        /// <summary>
        ///   Gets the name of the <see cref = "FubuRegistry" />. Mostly used for diagnostics.
        /// </summary>
        public virtual string Name
        {
            get { return GetType().ToString(); }
        }

        /// <summary>
        ///   Constructs a <see cref = "BehaviorGraph" /> using the configuration expressions defined in this <see cref = "FubuRegistry" />. This method is mostly for internal usage.
        /// </summary>
        /// <returns></returns>
        public BehaviorGraph BuildGraph()
        {
            Compile();

            if (_diagnosticLevel == DiagnosticLevel.FullRequestTracing)
            {
                Import(new DiagnosticsRegistry(), string.Empty);
            }

            // LATER, do this in such a way that it marks the provenance
            Import<AssetServicesRegistry>();

            var graph = new BehaviorGraph();
            _configuration.Build(graph);

            return graph;
        }

        internal void Compile()
        {
            if (!_hasCompiled)
            {
                _scanningOperations.Each(x => x(_configuration.Types));
                _hasCompiled = true;
            }
        }

        /// <summary>
        ///   Expression builder for configuring the default <see cref = "UrlPolicy" />, the default route, as well as supplying custom <see cref = "IUrlPolicy" /> implementations.
        /// </summary>
        public RouteConventionExpression Routes
        {
            get { return new RouteConventionExpression(Configuration); }
        }

        /// <summary>
        ///   Entry point to configure view attachment, conventions, and engines
        /// </summary>
        public ViewExpression Views
        {
            get { return new ViewExpression(_configuration, this); }
        }

        /// <summary>
        ///   Expression builder for configuring conventions that execute near the end of the build up of the <see cref = "BehaviorGraph" />.
        ///   These are useful when conditionally applying conventions use criteria like route patterns, input/output models, etc
        /// </summary>
        public PoliciesExpression Policies
        {
            get { return new PoliciesExpression(_configuration); }
        }

        /// <summary>
        ///   Entry point to configuring model binding
        /// </summary>
        public ModelsExpression Models
        {
            get { return new ModelsExpression(Services); }
        }

        /// <summary>
        ///   Expression builder for configuring the assemblies to include within the scanning operations used to produce the <see cref = "BehaviorGraph" />
        /// </summary>
        public AppliesToExpression Applies
        {
            get { return new AppliesToExpression(_configuration.Types); }
        }


        /// <summary>
        ///   Entry point to configuring how actions are found. Actions are the nuclei of behavior chains.
        /// </summary>
        public ActionCallCandidateExpression Actions
        {
            get { return new ActionCallCandidateExpression(_methodFilter, _configuration); }
        }

        /// <summary>
        ///   Expression builder for configuring content negotiation
        /// </summary>
        public MediaExpression Media
        {
            get { return new MediaExpression(this); }
        }

        /// <summary>
        ///   Configures the <see cref = "IServiceRegistry" /> to specify dependencies. 
        ///   This is an IoC-agnostic method of dependency configuration that will be consumed by the underlying implementation (e.g., StructureMap)
        /// </summary>
        public void Services(Action<IServiceRegistry> configure)
        {
            var registry = new ServiceRegistry();
            configure(registry);
            _configuration.AddServices(registry);
        }

        /// <summary>
        ///   Adds a configuration convention to be applied to the <see cref = "BehaviorGraph" /> produced by this <see cref = "FubuRegistry" />
        /// </summary>
        public void ApplyConvention<TConvention>()
            where TConvention : IConfigurationAction, new()
        {
            ApplyConvention(new TConvention());
        }

        /// <summary>
        ///   Adds a configuration convention to be applied to the <see cref = "BehaviorGraph" /> produced by this <see cref = "FubuRegistry" />
        /// </summary>
        public void ApplyConvention<TConvention>(TConvention convention)
            where TConvention : IConfigurationAction
        {
            _configuration.AddConvention(convention);
        }

        /// <summary>
        ///   Expression builder for defining and configuring a <see cref = "BehaviorChain" /> for a specific route
        /// </summary>
        public ExplicitRouteConfiguration.ChainedBehaviorExpression Route(string pattern)
        {
            var expression = new ExplicitRouteConfiguration(pattern);
            _configuration.AddExplicit(expression);

            return expression.Chain();
        }

        /// <summary>
        ///   Imports the specified <see cref = "FubuRegistry" />. 
        ///   Use a prefix to prefix routes generated by the registry.
        /// </summary>
        public void Import<T>(string prefix) where T : FubuRegistry, new()
        {
            _configuration.AddImport(new RegistryImport{
                Prefix = prefix,
                Registry = new T()
            });
        }

        /// <summary>
        ///   Imports the specified <see cref = "FubuRegistry" />. 
        ///   Use a prefix to prefix routes generated by the registry
        /// </summary>
        public void Import(FubuRegistry registry, string prefix)
        {
            _configuration.AddImport(new RegistryImport{
                Prefix = prefix,
                Registry = registry
            });
        }

        /// <summary>
        ///   Specifies whether to include diagnostics tracing. This is turned off by default
        /// </summary>
        public void IncludeDiagnostics(bool shouldInclude)
        {
            if (shouldInclude)
            {
                IncludeDiagnostics(config =>
                {
                    config.LimitRecordingTo(50);
                    config.ExcludeRequests(
                        r =>
                        r.Path != null &&
                        r.Path.ToLower().StartsWith("/{0}".ToFormat(DiagnosticUrlPolicy.DIAGNOSTICS_URL_ROOT)));
                });
            }
            else
            {
                _diagnosticLevel = DiagnosticLevel.None;
            }
        }

        /// <summary>
        ///   Configures the internal diagnostics for the <see cref = "BehaviorGraph" /> produced by this <see cref = "FubuRegistry" />.
        /// </summary>
        /// <param name = "configure"></param>
        public void IncludeDiagnostics(Action<IDiagnosticsConfigurationExpression> configure)
        {
            _diagnosticLevel = DiagnosticLevel.FullRequestTracing;

            var filters = new List<IRequestHistoryCacheFilter>();
            var config = new DiagnosticsConfigurationExpression(filters);
            configure(config);

            Services(graph =>
            {
                graph.SetServiceIfNone(new DiagnosticsIndicator().SetEnabled());
                graph.SetServiceIfNone(new DiagnosticsConfiguration{
                    MaxRequests = config.MaxRequests
                });
                filters
                    .Each(filter => graph.AddService(typeof (IRequestHistoryCacheFilter), new ObjectDef{
                        Type = filter.GetType(),
                        Value = filter
                    }));
            });
        }

        /// <summary>
        ///   Gets the level of diagnostics specified for this <see cref = "FubuRegistry" />
        /// </summary>
        public DiagnosticLevel DiagnosticLevel
        {
            get { return _diagnosticLevel; }
        }

        /// <summary>
        ///   Allows you to directly manipulate the BehaviorGraph produced by this FubuRegistry.
        ///   This should only be used after careful consideration and subsequent rejection of all other entry points to configuring the runtime
        ///   behaviour.
        /// </summary>
        public void Configure(Action<BehaviorGraph> alteration)
        {
            addExplicit(alteration);
        }

        private void addExplicit(Action<BehaviorGraph> action)
        {
            var explicitAction = new LambdaConfigurationAction(action);
            _configuration.AddExplicit(explicitAction);
        }


        /// <summary>
        ///   Access the TypePool with all the assemblies represented in the AppliesTo expressions
        ///   to make conventional registrations of any kind
        /// </summary>
        /// <param name = "configuration"></param>
        public void WithTypes(Action<TypePool> configuration)
        {
            _scanningOperations.Add(configuration);
        }


        /// <summary>
        ///   Only useful for testing scenarios.  Builds a BehaviorGraph without
        ///   the built in policies and services.
        /// </summary>
        /// <returns></returns>
        public BehaviorGraph BuildLightGraph()
        {
            return _configuration.BuildForImport(ViewBag.Empty());
        }

        public void Services<T>() where T : IServiceRegistry, new()
        {
            _configuration.AddServices(new T());
        }

        /// <summary>
        ///   Imports an IFubuRegistryExtension. The most
        ///   prominent Extensions you will care to add are those that set up
        ///   a view engine for you to use e.g. the WebFormsEngine or the
        ///   SparkEngine
        /// </summary>
        public void Import<T>() where T : IFubuRegistryExtension, new()
        {
            if (_importedTypes.Contains(typeof (T))) return;

            new T().Configure(this);

            _importedTypes.Add(typeof (T));
        }

        /// <summary>
        ///   Imports the declarations of an IFubuRegistryExtension
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        public void Import<T>(Action<T> configuration) where T : IFubuRegistryExtension, new()
        {
            var extension = new T();
            configuration(extension);

            extension.Configure(this);

            _importedTypes.Add(typeof (T));
        }
    }
}