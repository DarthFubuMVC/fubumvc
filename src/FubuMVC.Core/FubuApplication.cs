using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core
{
    // PLEASE NOTE:  This code is primarily tested with the StoryTeller suite for Packaging
    public class FubuApplication : IContainerFacilityExpression
    {
        private readonly FubuRegistry _registry;
        private Func<IContainerFacility> _facilitySource;
        private readonly List<Action<IPackageFacility>> _packagingDirectives = new List<Action<IPackageFacility>>();

        private FubuApplication(FubuRegistry registry)
        {
            _registry = registry;
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
            _facilitySource = () =>
            {
                _facility = facilitySource();
                return _facility;
            };
            return this;
        }

        public static IContainerFacilityExpression For(FubuRegistry registry)
        {
            return new FubuApplication(registry);
        }

        public static IContainerFacilityExpression For<T>() where T : FubuRegistry, new()
        {
            return For(new T());
        }

        private IContainerFacility _facility;

        [SkipOverForProvenance]
        public void Bootstrap(ICollection<RouteBase> routes)
        {
            if (HttpContext.Current != null)
            {
                UrlContext.Live();
            }

            var fubuFacility = new FubuMvcPackageFacility();

            _registry.Services(fubuFacility.RegisterServices);

            // TODO -- would be nice if this little monster also logged 
            PackageRegistry.LoadPackages(x =>
            {
                x.Facility(fubuFacility);
                _packagingDirectives.Each(d => d(x));
                x.Bootstrap(log => startApplication(routes));
            });

            fubuFacility.AddPackagingContentRoutes(routes);
        }

        private IEnumerable<IActivator> startApplication(ICollection<RouteBase> routes)
        {
            FindAllExtensions().Each(x => x.Configure(_registry));

            var facility = _facilitySource();

            // "Bake" the fubu configuration model into your
            // IoC container for the application
            var graph = _registry.BuildGraph();
            graph.EachService(facility.Register);
            var factory = facility.BuildFactory();

            // Register all the Route objects into the routes 
            // collection

            // TODO -- need a way to do this with debugging
            graph.VisitRoutes(x =>
            {
                x.Actions += (routeDef, chain) =>
                {
                    var route = routeDef.ToRoute();
                    route.RouteHandler = new FubuRouteHandler(factory, chain.UniqueId);

                    routes.Add(route);
                };
            });

            return facility.GetAllActivators();
        }


        public FubuApplication Packages(Action<IPackageFacility> configure)
        {
            _packagingDirectives.Add(configure);
            return this;
        }

        public FubuApplication ModifyRegistry(Action<FubuRegistry> modifications)
        {
            modifications(_registry);
            return this;
        }

        public static IEnumerable<IFubuRegistryExtension> FindAllExtensions()
        {
            if (!PackageRegistry.PackageAssemblies.Any()) return new IFubuRegistryExtension[0];

            var pool = new TypePool(null)
            {
                ShouldScanAssemblies = true
            };
            pool.AddAssemblies(PackageRegistry.PackageAssemblies);

            // Yeah, it really does have to be this way
            return pool.TypesMatching(
                t =>
                hasDefaultCtor(t) && t.GetInterfaces().Any(i => i.FullName == typeof(IFubuRegistryExtension).FullName))
                .Select(buildExtension);
        }

        private static bool hasDefaultCtor(Type type)
        {
            return type.GetConstructor(new Type[0]) != null;
        }

        private static IFubuRegistryExtension buildExtension(Type type)
        {
            var contextType = Type.GetType(type.AssemblyQualifiedName);
            return (IFubuRegistryExtension)Activator.CreateInstance(contextType);
        }

        public IContainerFacility Facility
        {
            get { return _facility; }
        }
    }

    public interface IContainerFacilityExpression
    {
        FubuApplication ContainerFacility(IContainerFacility facility);
        FubuApplication ContainerFacility(Func<IContainerFacility> facilitySource);
    }


}