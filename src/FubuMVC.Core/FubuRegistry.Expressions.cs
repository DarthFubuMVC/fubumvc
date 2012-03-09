using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core
{
    public partial class FubuRegistry : IFubuRegistry
    {
        private DiagnosticLevel _diagnosticLevel = DiagnosticLevel.None;
        private bool _diagnosticsRegistryImported = false;

        /// <summary>
        /// Expression builder for configuring the default <see cref="UrlPolicy"/>, the default route, as well as supplying custom <see cref="IUrlPolicy"/> implementations.
        /// </summary>
        public RouteConventionExpression Routes
        {
            get { return new RouteConventionExpression(_routeResolver, this); }
        }

        /// <summary>
        /// Entry point to configuring the output of behavior chains
        /// </summary>
        public OutputDeterminationExpression Output
        {
            get { return new OutputDeterminationExpression(this); }
        }

        /// <summary>
        /// Entry point to configure view attachment, conventions, and engines
        /// </summary>
        public ViewExpression Views
        {
            get { return new ViewExpression(_bagRunner, this, _viewAttacherConvention); }
        }

        /// <summary>
        /// Expression builder for configuring conventions that execute near the end of the build up of the <see cref="BehaviorGraph"/>.
        /// These are useful when conditionally applying conventions use criteria like route patterns, input/output models, etc
        /// </summary>
        public PoliciesExpression Policies
        {
            get { return new PoliciesExpression(_policies, _systemPolicies); }
        }

        /// <summary>
        /// Entry point to configuring model binding
        /// </summary>
        public ModelsExpression Models
        {
            get { return new ModelsExpression(Services); }
        }

        /// <summary>
        /// Expression builder for configuring the assemblies to include within the scanning operations used to produce the <see cref="BehaviorGraph"/>
        /// </summary>
        public AppliesToExpression Applies
        {
            get { return new AppliesToExpression(_types); }
        }

        
        private readonly ActionMethodFilter _methodFilter = new ActionMethodFilter();
        /// <summary>
        /// Entry point to configuring how actions are found. Actions are the nuclei of behavior chains.
        /// </summary>
        public ActionCallCandidateExpression Actions
        {
            get { return new ActionCallCandidateExpression(_actionSources, _methodFilter); }
        }

        /// <summary>
        /// Expression builder for configuring content negotiation
        /// </summary>
        public MediaExpression Media
        {
            get
            {
                return new MediaExpression(this, _connegAttachmentPolicy);
            }
        }

        /// <summary>
        /// Specify a custom <see cref="IConfigurationObserver"/> to use for all logging and diagnostics
        /// </summary>
        public void UsingObserver(IConfigurationObserver observer)
        {
            _observer = observer;
        }

        /// <summary>
        /// Configures the <see cref="IServiceRegistry"/> to specify dependencies. 
        /// This is an IoC-agnostic method of dependency configuration that will be consumed by the underlying implementation (e.g., StructureMap)
        /// </summary>
        public void Services(Action<IServiceRegistry> configure)
        {
            var registry = new ServiceRegistry();
            configure(registry);
            _serviceRegistrations.Add(registry);
        }

        /// <summary>
        /// Adds a configuration convention to be applied to the <see cref="BehaviorGraph"/> produced by this <see cref="FubuRegistry"/>
        /// </summary>
        public void ApplyConvention<TConvention>()
            where TConvention : IConfigurationAction, new()
        {
            ApplyConvention(new TConvention());
        }

        /// <summary>
        /// Adds a configuration convention to be applied to the <see cref="BehaviorGraph"/> produced by this <see cref="FubuRegistry"/>
        /// </summary>
        public void ApplyConvention<TConvention>(TConvention convention)
            where TConvention : IConfigurationAction
        {
            _conventions.Add(convention);
        }
        
        /// <summary>
        /// Expression builder for defining and configuring a <see cref="BehaviorChain"/> for a specific route
        /// </summary>
        public ChainedBehaviorExpression Route(string pattern)
        {
            var expression = new ExplicitRouteConfiguration(pattern);
            _explicits.Add(expression);

            return expression.Chain();
        }

        /// <summary>
        /// Imports the specified <see cref="FubuRegistry"/>. 
        /// Use a prefix to prefix routes generated by the registry.
        /// </summary>
        public void Import<T>(string prefix) where T : FubuRegistry, new()
        {
            if (_imports.Any(x => x.Registry is T)) return;

            Import(new T(), prefix);
        }

        /// <summary>
        /// Imports an IFubuRegistryExtension. The most
        /// prominent Extensions you will care to add are those that set up
        /// a view engine for you to use e.g. the WebFormsEngine or the
        /// SparkEngine
        /// </summary>
        public void Import<T>() where T : IFubuRegistryExtension, new()
        {
            if (_importedTypes.Contains(typeof(T))) return;

            new T().Configure(this);

            _importedTypes.Add(typeof(T));
        }

        /// <summary>
        /// Imports the declarations of an IFubuRegistryExtension
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Import<T>(Action<T> configuration) where T : IFubuRegistryExtension, new()
        {
            var extension = new T();
            configuration(extension);

            extension.Configure(this);

            _importedTypes.Add(typeof(T));
        }

        private readonly IList<Type> _importedTypes = new List<Type>();

        /// <summary>
        /// Imports the specified <see cref="FubuRegistry"/>. 
        /// Use a prefix to prefix routes generated by the registry
        /// </summary>
        public void Import(FubuRegistry registry, string prefix)
        {
            _imports.Add(new RegistryImport{
                Prefix = prefix,
                Registry = registry
            });
        }

        /// <summary>
        /// Adds a convention to execute against imported registries, providing access to the <see cref="FubuRegistry"/> being imported
        /// </summary>
        /// <param name="configuration"><see cref="FubuRegistry"/> parameter is the registry being imported</param>
        public void ConfigureImports(Action<FubuRegistry> configuration)
        {
            _importsConventions.Add(configuration);
        }

        /// <summary>
        /// Specifies whether to include diagnostics tracing. This is turned off by default
        /// </summary>
        public void IncludeDiagnostics(bool shouldInclude)
        {
            if (shouldInclude)
            {
                IncludeDiagnostics(config =>
                {
                    config.LimitRecordingTo(50);
                    config.ExcludeRequests(r => r.Path != null && r.Path.ToLower().StartsWith("/{0}".ToFormat(DiagnosticUrlPolicy.DIAGNOSTICS_URL_ROOT)));
                });
                ConfigureImports(i =>
                {
                    if (i is DiagnosticsRegistry) return;
                    i._diagnosticsRegistryImported = _diagnosticsRegistryImported;
                    i.IncludeDiagnostics(shouldInclude);
                });
            }
            else
            {
                _diagnosticLevel = DiagnosticLevel.None;
            }
        }

        /// <summary>
        /// Configures the internal diagnostics for the <see cref="BehaviorGraph"/> produced by this <see cref="FubuRegistry"/>.
        /// </summary>
        /// <param name="configure"></param>
        public void IncludeDiagnostics(Action<IDiagnosticsConfigurationExpression> configure)
        {
            _diagnosticLevel = DiagnosticLevel.FullRequestTracing;
            UsingObserver(new RecordingConfigurationObserver());
            if (!_diagnosticsRegistryImported)
            {
                Import<DiagnosticsRegistry>(string.Empty);
                _diagnosticsRegistryImported = true;
            }
            var filters = new List<IRequestHistoryCacheFilter>();
            var config = new DiagnosticsConfigurationExpression(filters);
            configure(config);

            Services(graph =>
            {
                graph.SetServiceIfNone(new DiagnosticsIndicator().SetEnabled());
                graph.SetServiceIfNone(new DiagnosticsConfiguration { MaxRequests = config.MaxRequests });
                filters
                    .Each(filter => graph.AddService(typeof(IRequestHistoryCacheFilter), new ObjectDef
                                                                                            {
                                                                                                Type = filter.GetType(),
                                                                                                Value = filter
                                                                                            }));
            });
        }

        /// <summary>
        /// Gets the level of diagnostics specified for this <see cref="FubuRegistry"/>
        /// </summary>
        public DiagnosticLevel DiagnosticLevel
        {
            get
            {
                return _diagnosticLevel;
            }
        }

        /// <summary>
        /// Allows you to directly manipulate the BehaviorGraph produced by this FubuRegistry.
        /// This should only be used after careful consideration and subsequent rejection of all other entry points to configuring the runtime
        /// behaviour. 
        /// </summary>
        public void Configure(Action<BehaviorGraph> alteration)
        {
            addExplicit(alteration);
        }


        /// <summary>
        /// Modify or configure the settings, data, and policies of the Assets Pipeline
        /// </summary>
        public AssetsExpression Assets
        {
            get
            {
                return new AssetsExpression(this);
            }
        }

        #region Nested type: RegistryImport

        public class RegistryImport
        {
            public string Prefix { get; set; }
            public FubuRegistry Registry { get; set; }

            public void ImportInto(IChainImporter graph)
            {
                graph.Import(Registry.BuildLightGraph(), b =>
                {
                    b.PrependToUrl(Prefix);
                    b.Origin = Registry.Name;
                });
            }
        }

        #endregion
    }


}