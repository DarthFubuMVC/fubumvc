using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Registration;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using FubuMVC.Core.ServiceBus.Sagas;
using StructureMap;
using StructureMap.Configuration.DSL;
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
                _applicationAssembly = FubuApplication.FindTheCallingAssembly();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly">The primary assembly for this application used in type scanning conventions and policies</param>
        public FubuRegistry(Assembly assembly) 
        {
            _applicationAssembly = assembly;
            _config = new ConfigGraph(_applicationAssembly);
        }


        public string NodeName
        {
            set
            {
                _name = value;
                AlterSettings<ChannelGraph>(x => x.Name = value);
            }
            get
            {
                return _name;
            }
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

        /// <summary>
        ///   Configures the <see cref = "IServiceRegistry" /> to specify dependencies. 
        ///   This is an IoC-agnostic method of dependency configuration that will be consumed by the underlying implementation (e.g., StructureMap)
        /// </summary>
        public void Services(Action<ServiceRegistry> configure)
        {
            configure(_config.ApplicationServices);
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


        public void Services<T>() where T : ServiceRegistry, new()
        {
            _config.Add(new T());
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
            get
            {
                return new FeatureExpression(this);
            }
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
                Include(typeof(T));
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
            /// retrofitting FubuTransportation to existing 
            /// systems with a very different nomenclature
            /// than the defaults
            /// </summary>
            public void DisableDefaultHandlerSource()
            {
                _parent.Config.Handlers.HandlerSources.RemoveAll(x => x is DefaultHandlerSource);
            }
        }




        private Func<IContainer> _containerSource = () => new Container();

        public void StructureMap(IContainer existing)
        {
            if (existing == null) throw new ArgumentNullException("existing");

            _containerSource = () => existing;
        }

        public void StructureMap<T>() where T : Registry, new()
        {
            _containerSource = Container.For<T>;
        }

        public void StructureMap(Registry registry)
        {
            _containerSource = () => new Container(registry);
        }

        internal IContainer ToContainer()
        {
            return _containerSource == null ? new Container() : _containerSource();
        }

        /// <summary>
        /// A shortcut to programmatically set the NodeId
        /// Useful for testing or running multiples of the same
        /// configured Node on one box
        /// </summary>
        public string NodeId
        {
            set
            {
                AlterSettings<ChannelGraph>(x => x.NodeId = value);
            }
        }


        public void DefaultSerializer<T>() where T : IMessageSerializer, new()
        {
            AlterSettings<ChannelGraph>(graph => graph.DefaultContentType = new T().ContentType);
        }

        public void DefaultContentType(string contentType)
        {
            AlterSettings<ChannelGraph>(graph => graph.DefaultContentType = contentType);
        }


        public void SagaStorage<T>() where T : ISagaStorage, new()
        {
            AlterSettings<TransportSettings>(x => x.SagaStorageProviders.Add(new T()));
        }

        /// <summary>
        /// Enable the in memory transport
        /// </summary>
        public void EnableInMemoryTransport(Uri replyUri = null)
        {
            AlterSettings<TransportSettings>(x => x.EnableInMemoryTransport = true);

            if (replyUri != null)
            {
                AlterSettings<MemoryTransportSettings>(x => x.ReplyUri = replyUri);
            }
        }

        public PollingJobExpression Polling
        {
            get { return new PollingJobExpression(this); }
        }



        public HealthMonitoringExpression HealthMonitoring
        {
            get
            {
                return new HealthMonitoringExpression(this);
            }
        }


    }
}