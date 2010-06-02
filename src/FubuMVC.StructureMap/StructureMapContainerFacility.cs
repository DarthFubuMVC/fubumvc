using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Linq;
using StructureMap.Query;

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
            return GetBasicFubuContainer(x => { });
        }

        public static IContainer GetBasicFubuContainer(Action<ConfigurationExpression> containerConfiguration)
        {
            var container = new Container(containerConfiguration);
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
            });

            initialize_Singletons_to_work_around_StructureMap_GitHub_Issue_3();

            return this;
        }

        private void initialize_Singletons_to_work_around_StructureMap_GitHub_Issue_3()
        {
            // Remove this method when the issue is closed 
            // http://github.com/structuremap/structuremap/issues#issue/3
            var allSingletons = _container.Model.PluginTypes.Where(x => x.Lifecycle == InstanceScope.Singleton.ToString());
            Debug.WriteLine("Found singletons: " + allSingletons.Count());
            foreach (var pluginType in allSingletons)
            {
                var instance = _container.GetInstance(pluginType.PluginType);
                Debug.WriteLine("Initialized singleton in primary container: " + instance);
            }
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