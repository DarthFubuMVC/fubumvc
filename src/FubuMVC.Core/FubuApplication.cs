using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Packaging.Environment;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Routing;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core
{
    public enum DiagnosticLevel
    {
        None,
        FullRequestTracing
    }

    // PLEASE NOTE:  This code is primarily tested with the StoryTeller suite for Packaging
    public class FubuApplication : IContainerFacilityExpression
    {
        private readonly Lazy<IContainerFacility> _facility;
        private readonly List<Action<IPackageFacility>> _packagingDirectives = new List<Action<IPackageFacility>>();
        private readonly Func<FubuRegistry> _registryBuilder;
        private readonly List<Action<FubuRegistry>> _registryModifications = new List<Action<FubuRegistry>>();
        private DiagnosticLevel _diagnosticLevel = DiagnosticLevel.None;
        private Func<IContainerFacility> _facilitySource;
        private FubuMvcPackageFacility _fubuFacility;
        private FubuRegistry _registryCache;

        private FubuApplication(Func<FubuRegistry> registry)
        {
            _registryBuilder = registry;

            _facility = new Lazy<IContainerFacility>(() => _facilitySource());
        }

        FubuApplication IContainerFacilityExpression.ContainerFacility(IContainerFacility facility)
        {
            return registerContainerFacilitySource(() => facility);
        }

        FubuApplication IContainerFacilityExpression.ContainerFacility(Func<IContainerFacility> facilitySource)
        {
            return registerContainerFacilitySource(facilitySource);
        }

        public FubuApplication ApplyDiagnostics(bool applies)
        {
            _diagnosticLevel = applies ? DiagnosticLevel.FullRequestTracing : DiagnosticLevel.None;
            return this;
        }


        private FubuRegistry registry()
        {
            return _registryCache ?? (_registryCache = _registryBuilder());
        }

        private FubuApplication registerContainerFacilitySource(Func<IContainerFacility> facilitySource)
        {
            _facilitySource = facilitySource;
            return this;
        }

        public static IContainerFacilityExpression For(Func<FubuRegistry> registry)
        {
            return new FubuApplication(registry);
        }

        public static IContainerFacilityExpression For<T>() where T : FubuRegistry, new()
        {
            return For(() => new T());
        }

        [SkipOverForProvenance]
        public void Bootstrap(IList<RouteBase> routes)
        {
            Bootstrap().Each(routes.Add);
        }

        [SkipOverForProvenance]
        public IList<RouteBase> Bootstrap()
        {
            if (HttpContext.Current != null)
            {
                UrlContext.Live();
            }

            _fubuFacility = new FubuMvcPackageFacility();

            IBehaviorFactory factory = null;
            BehaviorGraph graph = null;

            PackageRegistry.LoadPackages(x =>
            {
                x.Facility(_fubuFacility);
                _packagingDirectives.Each(d => d(x));

                
                x.Bootstrap(log =>
                {
                    // container facility has to be spun up here
                    var containerFacility = _facility.Value;

                    registerServicesFromFubuFacility();

                    applyRegistryModifications();

                    applyFubuExtensionsFromPackages();

                    graph = buildBehaviorGraph();

                    bakeBehaviorGraphIntoContainer(graph, containerFacility);

                    // factory HAS to be spun up here.
                    factory = containerFacility.BuildFactory(_diagnosticLevel);
                    if (_diagnosticLevel == DiagnosticLevel.FullRequestTracing)
                    {
                        factory = new DiagnosticBehaviorFactory(factory, containerFacility);
                    }

                    return containerFacility.GetAllActivators();
                });
            });

            PackageRegistry.AssertNoFailures();

            return buildRoutes(factory, graph);
        }

        private static void bakeBehaviorGraphIntoContainer(BehaviorGraph graph, IContainerFacility containerFacility)
        {
            graph.EachService(containerFacility.Register);
        }

        private BehaviorGraph buildBehaviorGraph()
        {
            var graph = registry().BuildGraph();

            return graph;
        }

        private void registerServicesFromFubuFacility()
        {
            registry().Services(_fubuFacility.RegisterServices);
        }

        private void applyRegistryModifications()
        {
            _registryModifications.Each(m => m(registry()));
        }

        private IList<RouteBase> buildRoutes(IBehaviorFactory factory, BehaviorGraph graph)
        {
            var routes = new List<RouteBase>();

            // Build route objects from route definitions on graph + add packaging routes
            _facility.Value.Get<IRoutePolicy>().BuildRoutes(graph, factory).Each(routes.Add);
            _fubuFacility.AddPackagingContentRoutes(routes);

            return routes;
        }

        private void applyFubuExtensionsFromPackages()
        {
            FubuExtensionFinder.FindAllExtensions().Each(x1 => x1.Configure(registry()));
        }

        public FubuApplication Packages(Action<IPackageFacility> configure)
        {
            _packagingDirectives.Add(configure);
            return this;
        }

        public FubuApplication ModifyRegistry(Action<FubuRegistry> modifications)
        {
            _registryModifications.Add(modifications);
            return this;
        }

        public IEnumerable<IInstaller> GetAllInstallers()
        {
            return _facility.Value.GetAllInstallers();
        }
    }

    public interface IContainerFacilityExpression
    {
        FubuApplication ContainerFacility(IContainerFacility facility);
        FubuApplication ContainerFacility(Func<IContainerFacility> facilitySource);
    }
}