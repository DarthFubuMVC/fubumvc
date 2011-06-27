using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Routing;
using Bottles;
using Bottles.Environment;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Packaging;
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

        public IBehaviorFactory BuildFactory(DiagnosticLevel diagnosticLevel)
        {
            _registry.For<IBehaviorFactory>().Use<PartialBehaviorFactory>();
            _container.Configure(x => x.AddRegistry(_registry));

            if (diagnosticLevel == DiagnosticLevel.FullRequestTracing)
            {
                _container.Configure(x =>
                {
                    x.For<IActionBehavior>().EnrichAllWith((context, behavior) =>
                    {
                        if (behavior is BehaviorTracer || behavior is DiagnosticBehavior) return behavior;

                        var tracer = context.GetInstance<BehaviorTracer>();
                        tracer.Inner = behavior;

                        var diagnostics = context.GetInstance<DiagnosticBehavior>();
                        diagnostics.Inner = tracer;

                        return diagnostics;
                    });

                    //x.For<IDebugReport>().HybridHttpOrThreadLocalScoped().Use<DebugReport>();
                    x.For<IDebugDetector>().Use<DebugDetector>();
                });
            }

            if (_initializeSingletonsToWorkAroundSMBug)
            {
                initialize_Singletons_to_work_around_StructureMap_GitHub_Issue_3();
            }

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

        public T Get<T>() where T : class
        {
            return _container.GetInstance<T>();
        }

        public IEnumerable<IActivator> GetAllActivators()
        {
            return _container.GetAllInstances<IActivator>();
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

        private void initialize_Singletons_to_work_around_StructureMap_GitHub_Issue_3()
        {
            // Remove this method when the issue is closed 
            // http://github.com/structuremap/structuremap/issues#issue/3
            var allSingletons =
                _container.Model.PluginTypes.Where(x => x.Lifecycle == InstanceScope.Singleton.ToString());
            Debug.WriteLine("Found singletons: " + allSingletons.Count());
            foreach (var pluginType in allSingletons)
            {
                var instance = _container.GetInstance(pluginType.PluginType);
                Debug.WriteLine("Initialized singleton in primary container: " + instance);
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