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
        /// Expression builder for configuring the output of behavior chains
        /// </summary>
        public OutputDeterminationExpression Output
        {
            get { return new OutputDeterminationExpression(this); }
        }

        /// <summary>
        /// Expression bulder for configuring view attachment, conventions, and engines
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
        /// Expression builder for configuring model binding
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

        /// <summary>
        /// Expression builder for configuring the available action types
        /// </summary>
        public ActionCallCandidateExpression Actions
        {
            get { return new ActionCallCandidateExpression(_behaviorMatcher, _types, _actionSourceMatcher); }
        }

        /// <summary>
        /// Expression builder for configuring content negotiation
        /// </summary>
        public MediaExpression Media
        {
            get
            {
                return new MediaExpression(this);
            }
        }

        /// <summary>
        /// Configures the built-in <see cref="TypeResolver"/>. Mostly used to add instances of <see cref="ITypeResolverStrategy"/>
        /// </summary>
        /// <param name="configure"></param>
        public void ResolveTypes(Action<TypeResolver> configure)
        {
            Services(x =>
            {
                x.SetServiceIfNone<ITypeResolver>(new TypeResolver());
                var resolver = x.FindAllValues<ITypeResolver>().FirstOrDefault() as TypeResolver;

                configure(resolver);
            });
        }

        /// <summary>
        /// Specify a custom <see cref="IConfigurationObserver"/> to use for all logging and diagnostics
        /// </summary>
        /// <param name="observer"></param>
        public void UsingObserver(IConfigurationObserver observer)
        {
            _observer = observer;
        }

        /// <summary>
        /// Configures the <see cref="IServiceRegistry"/> to specify dependencies. 
        /// This is an IoC-agnostic method of dependency configuration that will be consumed by the underlying implementation (e.g., StructureMap)
        /// </summary>
        /// <param name="configure"></param>
        public void Services(Action<IServiceRegistry> configure)
        {
            _serviceRegistrations.Add(configure);
        }

        /// <summary>
        /// Adds a configuration convention to be applied to the <see cref="BehaviorGraph"/> produced by this <see cref="FubuRegistry"/>
        /// </summary>
        /// <typeparam name="TConvention"></typeparam>
        public void ApplyConvention<TConvention>()
            where TConvention : IConfigurationAction, new()
        {
            ApplyConvention(new TConvention());
        }

        /// <summary>
        /// Adds a configuration convention to be applied to the <see cref="BehaviorGraph"/> produced by this <see cref="FubuRegistry"/>
        /// </summary>
        /// <typeparam name="TConvention"></typeparam>
        /// <param name="convention"></param>
        public void ApplyConvention<TConvention>(TConvention convention)
            where TConvention : IConfigurationAction
        {
            _conventions.Add(convention);
        }
        
        /// <summary>
        /// Expression builder for defining and configuring a <see cref="BehaviorChain"/> for a specific route
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
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
        /// <typeparam name="T"></typeparam>
        /// <param name="prefix"></param>
        public void Import<T>(string prefix) where T : FubuRegistry, new()
        {
            if (_imports.Any(x => x.Registry is T)) return;

            Import(new T(), prefix);
        }

        /// <summary>
        /// Imports the declarations of an IFubuRegistryExtension
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Import<T>() where T : IFubuRegistryExtension, new()
        {
            new T().Configure(this);
        }

        /// <summary>
        /// Imports the specified <see cref="FubuRegistry"/>. 
        /// Use a prefix to prefix routes generated by the registry
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="prefix"></param>
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
        /// <param name="shouldInclude"></param>
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
                Actions
                    .ExcludeTypes(t => t.HasAttribute<FubuDiagnosticsAttribute>())
                    .ExcludeMethods(call => call.Method.HasAttribute<FubuDiagnosticsAttribute>());
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
        ///   This allows you to drop down to direct manipulation of the BehaviorGraph
        ///   produced by this FubuRegistry
        /// </summary>
        /// <param name = "alteration"></param>
        public void Configure(Action<BehaviorGraph> alteration)
        {
            addExplicit(alteration);
        }

        #region Nested type: RegistryImport

        public class RegistryImport
        {
            public string Prefix { get; set; }
            public FubuRegistry Registry { get; set; }

            public void ImportInto(IChainImporter graph)
            {
                graph.Import(Registry.BuildGraph(), b =>
                {
                    b.PrependToUrl(Prefix);
                    b.Origin = Registry.Name;
                });
            }
        }

        #endregion
    }


}