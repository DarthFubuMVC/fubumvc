using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bottles;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Configuration;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
using FubuMVC.StructureMap.Settings;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace FubuMVC.StructureMap
{
    public class StructureMapContainerFacility : IContainerFacility, IServiceFactory
    {
        private readonly Registry _registry;

        private readonly Stack<IContainer> _containers = new Stack<IContainer>(); 

        public StructureMapContainerFacility(IContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");

            _containers.Push(container);

            _registry = new StructureMapFubuRegistry();
            _registry.For<IServiceFactory>().Use(this);

            _registration = (serviceType, def) =>
            {
                if (serviceType == typeof (Registry))
                {
                    var registry = def.Value as Registry;
                    if (registry != null)
                    {
                        Container.Configure(x => x.IncludeRegistry(registry));
                    }

                    if (def.Type.CanBeCastTo<Registry>() && def.Type.IsConcreteWithDefaultCtor())
                    {
                        registry = (Registry) Activator.CreateInstance(def.Type);
                        Container.Configure(x => x.IncludeRegistry(registry));
                    }

                    return;
                }

                if (def.Value == null)
                {
                    _registry.For(serviceType).Add(new ObjectDefInstance(def));
                }
                else
                {
                    _registry.For(serviceType).Add(new ObjectInstance(def.Value)
                    {
                        Name = def.Name
                    });
                }

                if (ServiceRegistry.ShouldBeSingleton(serviceType) || ServiceRegistry.ShouldBeSingleton(def.Type) || def.IsSingleton)
                {
                    _registry.For(serviceType).Singleton();
                }
            };
        }

        public IContainer Container
        {
            get { return _containers.Peek(); }
        }

        public virtual IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId)
        {
            return new NestedStructureMapContainerBehavior(Container, arguments, behaviorId);
        }

        public T Build<T>(ServiceArguments arguments)
        {
            return Container.GetInstance<T>(arguments.ToExplicitArgs());
        }

        public IServiceFactory BuildFactory(BehaviorGraph graph)
        {
            Container.Configure(x => {
                x.AddRegistry(_registry);
                x.Policies.OnMissingFamily<SettingPolicy>();
            });

            _registration = (serviceType, def) =>
            {
                if (def.Value != null)
                {
                    Container.Configure(x => x.For(serviceType).Add(def.Value));
                }
                else
                {
                    Container.Configure(x => x.For(serviceType).Add(new ObjectDefInstance(def)));
                }

                
            };

            return this;
        }

        private Action<Type, ObjectDef> _registration; 

        public void Register(Type serviceType, ObjectDef def)
        {
            _registration(serviceType, def);
        }

        public void Shutdown()
        {
            Container.SafeDispose();
        }

        public T Get<T>()
        {
            return Container.GetInstance<T>();
        }

        public IEnumerable<T> GetAll<T>()
        {
            return Container.GetAllInstances<T>();
        }

        public static IContainer GetBasicFubuContainer()
        {
            return GetBasicFubuContainer(x => { });
        }

        public static IContainer GetBasicFubuContainer(Action<ConfigurationExpression> containerConfiguration)
        {
            var container = new Container(containerConfiguration);

            container.Configure(x => x.For<IHttpResponse>().Use(new OwinHttpResponse()));

            FubuApplication.For(() => new FubuRegistry()).StructureMap(container).Bootstrap();

            return container;
        }

        public void Dispose()
        {
            // DO NOTHING BECAUSE THIS CAUSED A STACKOVERFLOW TO DISPOSE THE CONTAINER HERE.
        }

        /// <summary>
        /// Creates a new StructureMap child container and makes that the new active container
        /// </summary>
        public void StartNewScope()
        {
            var child = Container.CreateChildContainer();
            _containers.Push(child);
        }

        /// <summary>
        /// Tears down any active child container and pops it out of the active container stack
        /// </summary>
        public void TeardownScope()
        {
            if (_containers.Count >= 2)
            {
                var child = _containers.Pop();
                child.Dispose();
            }
        }
    }

}