using System;
using System.Collections.Generic;
using System.Web.Routing;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace FubuMVC.StructureMap
{
    // TODO:  Container start up stuff?  Maybe just do it w/ behaviors
    public class StructureMapContainerFacility : IContainerFacility, IBehaviorFactory
    {
        private readonly IContainer _container;
        private readonly Registry _registry;

        public Func<IContainer, ServiceArguments, Guid, IActionBehavior> Builder =
            (container, args, behaviorId) => { return new NestedStructureMapContainerBehavior(container, args, behaviorId); };


        public static IContainer GetBasicFubuContainer()
        {
            var container = new Container();
            var facility = new StructureMapContainerFacility(container);
            new FubuBootstrapper(facility, new FubuRegistry()).Bootstrap(new List<RouteBase>());

            return container;
        }

        public StructureMapContainerFacility(IContainer container)
        {
            _container = container;
            _registry = new StructureMapFubuRegistry();
        }

        public IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId)
        {
            return Builder(_container, arguments, behaviorId);
        }

        public IBehaviorFactory BuildFactory()
        {
            _registry.For<IBehaviorFactory>().Use<PartialBehaviorFactory>();
            _container.Configure(x =>
            {
                x.AddRegistry(_registry);


                // TEMPORARY, I THINK
                x.For<ISessionState>().LifecycleIs(new HttpSessionLifecycle()).Use<BasicSessionState>();
            });

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
                _registry.For(serviceType).Add(new ObjectInstance(def.Value));
            }

            if (ServiceRegistry.ShouldBeSingleton(serviceType))
            {
                _registry.For(serviceType).Singleton();
            }
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