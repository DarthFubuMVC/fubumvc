using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.StructureMap.Settings;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace FubuMVC.Core.StructureMap
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

            _registration = (serviceType, instance) =>
            {
                if (ServiceRegistry.ShouldBeSingleton(serviceType) || ServiceRegistry.ShouldBeSingleton(instance.ReturnedType))
                {
                    instance.SetLifecycleTo<SingletonLifecycle>();
                }

                _registry.For(serviceType).Add(instance);
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

            _registration = (serviceType, instance) =>
            {
                Container.Configure(x => x.For(serviceType).Add(instance));
            };

            return this;
        }

        private Action<Type, Instance> _registration; 

        public void Register(Type serviceType, Instance instance)
        {
            _registration(serviceType, instance);
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

            var registry = new FubuRegistry();
            registry.StructureMap(container);

            FubuApplication.For(registry).Bootstrap();

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