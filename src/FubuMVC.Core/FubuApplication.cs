using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Routing;
using Bottles;
using Bottles.Services;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Http;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Routing;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core
{
    // PLEASE NOTE:  This code is primarily tested through the integration tests

    /// <summary>
    /// Key class used to define and bootstrap a FubuMVC application
    /// </summary>
    public class FubuApplication : IContainerFacilityExpression, IApplication<FubuRuntime>
    {
        private readonly Lazy<IContainerFacility> _facility;
        private readonly List<Action<IPackageFacility>> _packagingDirectives = new List<Action<IPackageFacility>>();
        private readonly Lazy<FubuRegistry> _registry;
        private Func<IContainerFacility> _facilitySource;
        private FubuMvcPackageFacility _fubuFacility;

        private FubuApplication(Func<FubuRegistry> registryBuilder)
        {
            _registry = new Lazy<FubuRegistry>(registryBuilder);
            _facility = new Lazy<IContainerFacility>(() => _facilitySource());
        }

        public IContainerFacility Facility
        {
            get
            {
                if (!_facility.IsValueCreated)
                {
                    throw new InvalidOperationException(
                        "Application has not yet been bootstrapped.  This operation is only valid after bootstrapping the application");
                }


                return _facility.Value;
            }
        }

        FubuApplication IContainerFacilityExpression.ContainerFacility(IContainerFacility facility)
        {
            return registerContainerFacilitySource(() => facility);
        }

        FubuApplication IContainerFacilityExpression.ContainerFacility(Func<IContainerFacility> facilitySource)
        {
            return registerContainerFacilitySource(facilitySource);
        }

        private FubuApplication registerContainerFacilitySource(Func<IContainerFacility> facilitySource)
        {
            _facilitySource = facilitySource;
            return this;
        }

        /// <summary>
        /// Use the policies and conventions specified by the FubuRegistry built by the specified lambda.  
        /// Use this overload if the FubuRegistry type needs to use the scanned Bottle assemblies and packages 
        /// hanging off of PackageRegistry
        /// </summary>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static IContainerFacilityExpression For(Func<FubuRegistry> registry)
        {
            return new FubuApplication(registry);
        }

        /// <summary>
        /// Use the policies and conventions specified by the registry
        /// </summary>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static IContainerFacilityExpression For(FubuRegistry registry)
        {
            return new FubuApplication(() => registry);
        }

        /// <summary>
        /// Use only the default FubuMVC policies and conventions
        /// </summary>
        /// <returns></returns>
        public static IContainerFacilityExpression DefaultPolicies()
        {
            var assembly = TypePool.FindTheCallingAssembly();
            return new FubuApplication(() => new FubuRegistry(assembly));
        }

        /// <summary>
        /// Use the policies and conventions specified by a specific FubuRegistry of type "T"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IContainerFacilityExpression For<T>() where T : FubuRegistry, new()
        {
            return For(() => new T());
        }

        /// <summary>
        /// Shortcut method to immediately bootstrap the specified FubuMVC application
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FubuRuntime BootstrapApplication<T>() where T : IApplicationSource, new()
        {
            return new T().BuildApplication().Bootstrap();
        }

        /// <summary>
        /// Called to bootstrap and "start" a FubuMVC application 
        /// </summary>
        /// <returns></returns>
        [SkipOverForProvenance]
        public FubuRuntime Bootstrap()
        {
            SetupNamingStrategyForHttpHeaders();

            _fubuFacility = new FubuMvcPackageFacility();

            IServiceFactory factory = null;
            BehaviorGraph graph = null;

            // TODO -- I think Bottles probably needs to enforce a "tell me the paths"
            // step maybe
            PackageRegistry.GetApplicationDirectory = FubuMvcPackageFacility.GetApplicationPath;
            BottleFiles.ContentFolder = FubuMvcPackageFacility.FubuContentFolder;
            BottleFiles.PackagesFolder = FileSystem.Combine("bin", FubuMvcPackageFacility.FubuPackagesFolder);

            IList<RouteBase> routes = null;

            PackageRegistry.LoadPackages(x => {
                x.Facility(_fubuFacility);
                _packagingDirectives.Each(d => d(x));

                x.Bootstrap(log => {
                    // container facility has to be spun up here
                    var containerFacility = _facility.Value;

                    // Need to do this to make the provenance for bottles come out right
                    _registry.Value.Config.Seal();

                    applyFubuExtensionsFromPackages();

                    graph = buildBehaviorGraph();

                    bakeBehaviorGraphIntoContainer(graph, containerFacility);

                    // factory HAS to be spun up here.
                    factory = containerFacility.BuildFactory();

                    routes = buildRoutes(factory, graph);
                    routes.Each(r => RouteTable.Routes.Add(r));
                    containerFacility.Register(typeof(FubuRouteTable), ObjectDef.ForValue(new FubuRouteTable{Routes = routes}));

                    return factory.GetAll<IActivator>();
                });
            });

            FubuMvcPackageFacility.Restarted = DateTime.Now;

            PackageRegistry.AssertNoFailures(
                () => { throw new FubuException(0, FubuApplicationDescriber.WriteDescription()); });


            var runtime = new FubuRuntime(factory, _facility.Value, routes);

            _facility.Value.Register(typeof(FubuRuntime), ObjectDef.ForValue(runtime));

            return runtime;
        }

        public static void SetupNamingStrategyForHttpHeaders()
        {
            BindingContext.AddNamingStrategy(HttpRequestHeaders.HeaderDictionaryNameForProperty);
        }

        private void bakeBehaviorGraphIntoContainer(BehaviorGraph graph, IContainerFacility containerFacility)
        {
            graph.As<IRegisterable>().Register(containerFacility.Register);
        }

        private BehaviorGraph buildBehaviorGraph()
        {
            var graph = BehaviorGraphBuilder.Build(_registry.Value);

            return graph;
        }

        private IList<RouteBase> buildRoutes(IServiceFactory factory, BehaviorGraph graph)
        {
            var routes = new List<RouteBase>();

            // Build route objects from route definitions on graph + add packaging routes
            factory.Get<IRoutePolicy>().BuildRoutes(graph, factory).Each(routes.Add);

            return routes;
        }

        private void applyFubuExtensionsFromPackages()
        {
            PackageRegistry.Diagnostics.EachLog((o, l) => {
                if (o is IPackageInfo)
                {
                    _registry.Value.Config.Push(o.As<IPackageInfo>());
                    var assemblies = l.FindChildren<Assembly>();

                    try
                    {
                        FubuExtensionFinder.ApplyExtensions(_registry.Value, assemblies, l);
                    }
                    catch (Exception e)
                    {
                        l.MarkFailure(e);
                    }

                    _registry.Value.Config.Pop();
                }
            });
        }

        /// <summary>
        /// Modify the Bottles configuration for this application
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public FubuApplication Packages(Action<IPackageFacility> configure)
        {
            _packagingDirectives.Add(configure);
            return this;
        }
    }

    public class FubuRouteTable
    {
        public IList<RouteBase> Routes;
    }
}