using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.UI.Navigation;

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
        private readonly ConfigurationGraph _configuration;
        private readonly IList<Type> _importedTypes = new List<Type>();
        private readonly ActionMethodFilter _methodFilter = new ActionMethodFilter();
        private readonly IList<Action<TypePool>> _scanningOperations = new List<Action<TypePool>>();

        private bool _hasCompiled;

        public FubuRegistry()
        {
            _configuration = new ConfigurationGraph(this);
        }

        public FubuRegistry(Action<FubuRegistry> configure) : this()
        {
            configure(this);
        }

        internal ConfigurationGraph Configuration
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
            _configuration.AddConfiguration(registry);
        }

        /// <summary>
        ///   Adds a configuration convention to be applied to the <see cref = "BehaviorGraph" /> produced by this <see cref = "FubuRegistry" />
        /// </summary>
        public void ApplyConvention<TConvention>(Action<TConvention> configuration = null)
            where TConvention : IConfigurationAction, new()
        {
            var convention = new TConvention();
            if (configuration != null)
            {
                configuration(convention);
            }

            ApplyConvention(convention);
        }

        /// <summary>
        ///   Adds a configuration convention to be applied to the <see cref = "BehaviorGraph" /> produced by this <see cref = "FubuRegistry" />
        /// </summary>
        public void ApplyConvention<TConvention>(TConvention convention)
            where TConvention : IConfigurationAction
        {
            _configuration.AddConfiguration(convention, ConfigurationType.Discovery);
        }

        /// <summary>
        ///   Expression builder for defining and configuring a <see cref = "BehaviorChain" /> for a specific route
        /// </summary>
        public ExplicitRouteConfiguration.ChainedBehaviorExpression Route(string pattern)
        {
            var expression = new ExplicitRouteConfiguration(pattern);
            _configuration.AddConfiguration(expression, ConfigurationType.Explicit);

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
                Registry = new T(),
                Type = typeof (T)
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
        /// Allows you to manipulate a settings object on <see cref="BehaviorGraph.Settings"/>.
        /// </summary>
        public void AlterSettings<T>(Action<T> alteration) where T : new()
        {
            Configure(graph => graph.Settings.Alter(alteration));
        }

        /// <summary>
        /// Completely replace the setting object for type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings"></param>
        public void ReplaceSettings<T>(T settings)
        {
            Configure(graph => graph.Settings.Replace(settings));
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

        /// <summary>
        ///   Constructs a <see cref = "BehaviorGraph" /> using the configuration expressions defined in this <see cref = "FubuRegistry" />. This method is mostly for internal usage.
        /// </summary>
        /// <returns></returns>
        internal BehaviorGraph BuildGraph()
        {
            Compile();

            return _configuration.Build();
        }


        internal void Compile()
        {
            if (!_hasCompiled)
            {
                _scanningOperations.Each(x => x(_configuration.Types));
                _hasCompiled = true;
            }
        }

        private void addExplicit(Action<BehaviorGraph> action)
        {
            var explicitAction = new LambdaConfigurationAction(action);
            _configuration.AddConfiguration(explicitAction, ConfigurationType.Explicit);
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


        public void Services<T>() where T : IServiceRegistry, new()
        {
            _configuration.AddConfiguration((IConfigurationAction) new T(), ConfigurationType.Services);
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

            if (typeof (T).CanBeCastTo<FubuPackageRegistry>())
            {
                _configuration.AddImport(new RegistryImport{
                    Prefix = null,
                    Registry = new T().As<FubuRegistry>(),
                    Type = typeof (T)
                });
            }
            else
            {
                new T().Configure(this);
                _importedTypes.Add(typeof (T));
            }
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

        public void Navigation<T>() where T : NavigationRegistry, new()
        {
            _configuration.AddConfiguration(new T());
        }

        public void Navigation(Action<NavigationRegistry> configuration)
        {
            var registry = new NavigationRegistry();
            configuration(registry);

            _configuration.AddConfiguration(registry);
        }

        public void Navigation(NavigationRegistry registry)
        {
            _configuration.AddConfiguration(registry);
        }
    }
}