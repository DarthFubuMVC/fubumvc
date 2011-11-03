using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Environment;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace FubuMVC.StructureMap
{
    public class StructureMapContainerFacility : IContainerFacility, IBehaviorFactory
    {
        private readonly IContainer _container;
        private readonly Registry _registry;


        private bool _initializeSingletonsToWorkAroundSMBug = true;

        public StructureMapContainerFacility(IContainer container)
        {
            _container = container;
            _registry = new StructureMapFubuRegistry();
        }

        public IContainer Container
        {
            get { return _container; }
        }

        public virtual IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId)
        {
            return new NestedStructureMapContainerBehavior(_container, arguments, behaviorId);
        }

        public IBehaviorFactory BuildFactory()
        {
            _registry.For<IBehaviorFactory>().Use<PartialBehaviorFactory>();
            _container.Configure(x => x.AddRegistry(_registry));

            return this;
        }

        public void Register(Type serviceType, ObjectDef def)
        {
            if (def.Value == null)
            {
                _registry.For(serviceType).Add(new ObjectDefInstance(def));
            }
            else
            {
                _registry.For(serviceType).Add(new ObjectInstance(def.Value){
                    Name = def.Name
                });
            }

            if (ServiceRegistry.ShouldBeSingleton(serviceType))
            {
                _registry.For(serviceType).Singleton();
            }
        }

        public void Inject(Type abstraction, Type concretion)
        {
            _container.Configure(x => x.For(abstraction).Add(concretion));
        }

        public T Get<T>()
        {
            return _container.GetInstance<T>();
        }

        public IEnumerable<T> GetAll<T>()
        {
            return _container.GetAllInstances<T>();
        }

        public IEnumerable<IActivator> GetAllActivators()
        {  
            if (_initializeSingletonsToWorkAroundSMBug)
            {
                new SingletonSpinupActivator(_container).Activate(null, null);
            }
            foreach (var activator in _container.GetAllInstances<IActivator>())
            {
                yield return activator;
            }
        }

        public IEnumerable<IInstaller> GetAllInstallers()
        {
            return _container.GetAllInstances<IInstaller>();
        }

        public static IContainer GetBasicFubuContainer()
        {
            return GetBasicFubuContainer(x => { });
        }

        public static IContainer GetBasicFubuContainer(Action<ConfigurationExpression> containerConfiguration)
        {
            var container = new Container(containerConfiguration);

            container.Configure(x => x.For<IHttpWriter>().Use(new NulloHttpWriter()));

            FubuApplication.For(() => new FubuRegistry()).StructureMap(container).Bootstrap();

            return container;
        }

        /// <summary>
        ///   Disable FubuMVC's protection for a known StructureMap nested container issue. 
        ///   You will need to manually initialize any Singletons in Application_Start if they depend on instances scoped to a nested container.
        ///   See <see cref = "http://github.com/structuremap/structuremap/issues#issue/3" />
        /// </summary>
        /// <returns></returns>
        public StructureMapContainerFacility DoNotInitializeSingletons()
        {
            _initializeSingletonsToWorkAroundSMBug = false;
            return this;
        }

    }

    public class PartialBehaviorFactory : IBehaviorFactory
    {
        private readonly IContainer _container;

        public PartialBehaviorFactory(IContainer container)
        {
            _container = container;
        }

        public IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId)
        {
            return _container.GetInstance<IActionBehavior>(arguments.ToExplicitArgs(), behaviorId.ToString());
        }
    }
}