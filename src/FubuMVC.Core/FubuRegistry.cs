using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Registration;
using FubuMVC.Core.Services;
using StructureMap;
using PoliciesExpression = FubuMVC.Core.Registration.DSL.PoliciesExpression;

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
    public class FubuRegistry
    {
        private readonly IList<Type> _importedTypes = new List<Type>();
        private readonly Assembly _applicationAssembly;
        private readonly ConfigGraph _config;

        private string _name;

        public FubuRegistry()
        {
            var type = GetType();

            _name = type.Name.Replace("TransportRegistry", "").Replace("Registry", "").ToLower();

            if (type == typeof (FubuRegistry) || type == typeof (FubuPackageRegistry))
            {
                _applicationAssembly = AssemblyFinder.FindTheCallingAssembly();
            }
            else
            {
                _applicationAssembly = type.Assembly;
            }

            _config = new ConfigGraph(_applicationAssembly);

            if (!this.GetType().CanBeCastTo<FubuPackageRegistry>())
            {
                // TODO: Hokey. Gotta be a better way to do this
                AlterSettings<ChannelGraph>(x =>
                {
                    if (x.Name.IsEmpty())
                    {
                        x.Name = _name;
                    }
                });
            }
        }

        public FubuRegistry(Action<FubuRegistry> configure) : this()
        {
            configure(this);
        }

        public FubuRuntime ToRuntime()
        {
            return new FubuRuntime(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly">The primary assembly for this application used in type scanning conventions and policies</param>
        public FubuRegistry(Assembly assembly)
        {
            _applicationAssembly = assembly;
            _config = new ConfigGraph(_applicationAssembly);
        }

        /// <summary>
        /// If set, overrides the automatic determination of where FubuMVC loads
        /// content files like views, scripts, and stylesheets
        /// </summary>
        public string RootPath;

        /// <summary>
        /// Sets the RootPath to the root directory of a named folder parallel
        /// to the currently executing project
        /// </summary>
        /// <param name="directory"></param>
        public void UseParallelDirectory(string directory)
        {
            var path = FubuRuntime.DefaultApplicationPath();

            RootPath = path.ParentDirectory().AppendPath(directory);
        }


        public int Port = PortFinder.FindPort(5500);
        public IHost Host;

        public void HostWith<T>(int port = 0) where T : IHost, new()
        {
            Host = new T();
            if (port > 0) Port = port;
        }


        public string NodeName
        {
            set
            {
                _name = value;
                AlterSettings<ChannelGraph>(x => x.Name = value);
            }
            get { return _name; }
        }

        /// <summary>
        /// A shortcut to programmatically set the NodeId
        /// Useful for testing or running multiples of the same
        /// configured Node on one box
        /// </summary>
        public string NodeId
        {
            set { AlterSettings<ChannelGraph>(x => x.NodeId = value); }
        }

        internal ConfigGraph Config
        {
            get { return _config; }
        }

        /// <summary>
        ///   Gets the name of the <see cref = "FubuRegistry" />. Mostly used for diagnostics.
        /// </summary>
        public virtual string Name
        {
            get { return GetType().ToString(); }
        }


        /// <summary>
        ///   Expression builder for configuring conventions that execute near the end of the build up of the <see cref = "BehaviorGraph" />.
        ///   These are useful when conditionally applying conventions use criteria like route patterns, input/output models, etc
        /// </summary>
        public PoliciesExpression Policies
        {
            get { return new PoliciesExpression(_config); }
        }

        /// <summary>
        ///   Entry point to configuring model binding
        /// </summary>
        public ModelsExpression Models
        {
            get { return new ModelsExpression(Services); }
        }

        /// <summary>
        ///   Entry point to configuring how actions are found. Actions are the nuclei of behavior chains.
        /// </summary>
        public ActionCallCandidateExpression Actions
        {
            get { return new ActionCallCandidateExpression(_config); }
        }

        public ServiceRegistry Services
        {
            get { return _config.ApplicationServices; }
        }


        /// <summary>
        ///   Imports the specified <see cref = "FubuRegistry" />. 
        ///   Use a prefix to prefix routes generated by the registry.
        /// </summary>
        public void Import<T>(string prefix) where T : FubuRegistry, new()
        {
            _config.AddImport(new RegistryImport
            {
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
            _config.AddImport(new RegistryImport
            {
                Prefix = prefix,
                Registry = registry
            });
        }

        /// <summary>
        /// Allows you to manipulate a settings object on <see cref="BehaviorGraph.Settings"/>.
        /// </summary>
        public void AlterSettings<T>(Action<T> alteration) where T : class, new()
        {
            Config.Settings.Fill(new SettingAlteration<T>(alteration));
        }

        /// <summary>
        /// Completely replace the setting object for type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings"></param>
        public void ReplaceSettings<T>(T settings) where T : class
        {
            Config.Settings.Fill(new SettingReplacement<T>(settings));
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
        ///   Imports an IFubuRegistryExtension. The most
        ///   prominent Extensions you will care to add are those that set up
        ///   a view engine for you to use e.g. the WebFormsEngine or the
        ///   SparkEngine
        /// </summary>
        public void Import<T>() where T : IFubuRegistryExtension, new()
        {
            if (_importedTypes.Contains(typeof (T))) return;


            var extension = new T();

            extension.Configure(this);

            if (!typeof (T).CanBeCastTo<FubuPackageRegistry>())
            {
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


        /// <summary>
        /// Enable and configure optional features
        /// </summary>
        public FeatureExpression Features
        {
            get { return new FeatureExpression(this); }
        }


        private void addExplicit(Action<BehaviorGraph> action)
        {
            var explicitAction = new LambdaConfigurationAction(action);
            _config.Local.Explicits.Fill(explicitAction);
        }

        public Assembly ApplicationAssembly
        {
            get { return _applicationAssembly; }
        }

        public void RoutePolicy<T>() where T : IRoutePolicy, new()
        {
            Configure(x => x.RoutePolicy = new T());
        }


        public HandlersExpression Handlers
        {
            get { return new HandlersExpression(this); }
        }

        public class HandlersExpression
        {
            private readonly FubuRegistry _parent;

            public HandlersExpression(FubuRegistry parent)
            {
                _parent = parent;
            }

            public void Include(params Type[] types)
            {
                _parent.Config.Add(new ExplicitTypeHandlerSource(types));
            }

            public void Include<T>()
            {
                Include(typeof (T));
            }

            public void FindBy(Action<HandlerSource> configuration)
            {
                var source = new HandlerSource();
                configuration(source);

                _parent.Config.Add(source);
            }

            public void FindBy<T>() where T : IHandlerSource, new()
            {
                _parent.Config.Add(new T());
            }

            public void FindBy(IHandlerSource source)
            {
                _parent.Config.Add(source);
            }

            /// <summary>
            /// Completely remove the default handler finding
            /// logic.  This is probably only applicable to 
            /// retrofitting the ServiceBus feature to existing 
            /// systems with a very different nomenclature
            /// than the defaults
            /// </summary>
            public void DisableDefaultHandlerSource()
            {
                _parent.Config.Handlers.HandlerSources.RemoveAll(x => x is DefaultHandlerSource);
            }
        }


        private Func<IContainer> _containerSource = () => new Container();
        
        public string Mode = null;

        public void StructureMap(IContainer existing)
        {
            if (existing == null) throw new ArgumentNullException("existing");

            _containerSource = () => existing;
        }

        internal IContainer ToContainer()
        {
            return _containerSource == null ? new Container() : _containerSource();
        }


        public ServiceBusFeature ServiceBus
        {
            get { return new ServiceBusFeature(this); }
        }


        public PollingJobExpression Polling
        {
            get { return new PollingJobExpression(this); }
        }
    }
}