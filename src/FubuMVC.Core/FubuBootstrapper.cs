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
    [Obsolete("Please move to using the FubuApplication model instead")]
    public class FubuBootstrapper
    {

        private readonly IContainerFacility _facility;
        private readonly FubuRegistry _topRegistry;

        public FubuBootstrapper(IContainerFacility facility, FubuRegistry topRegistry)
        {
            _facility = facility;
            _topRegistry = topRegistry;
        }

        public void Bootstrap(ICollection<RouteBase> routes)
        {
            if (HttpContext.Current != null)
            {
                UrlContext.Live();
            }



            // Find all of the IFubuRegistryExtension's and apply
            // them to the top level FubuRegistry *BEFORE*
            // registering the Fubu application parts into
            // your IoC container
            FindAllExtensions().Each(x => x.Configure(_topRegistry));

            _facility.Activate(routes, _topRegistry);
        }


        public static IEnumerable<IFubuRegistryExtension> FindAllExtensions()
        {
            if (!PackageRegistry.PackageAssemblies.Any()) return new IFubuRegistryExtension[0];

            var pool = new TypePool(null){
                ShouldScanAssemblies = true
            };
            pool.AddAssemblies(PackageRegistry.PackageAssemblies);

            // Yeah, it really does have to be this way
            return pool.TypesMatching(
                t =>
                hasDefaultCtor(t) && t.GetInterfaces().Any(i => i.FullName == typeof (IFubuRegistryExtension).FullName))
                .Select(buildExtension);
        }

        private static bool hasDefaultCtor(Type type)
        {
            return type.GetConstructor(new Type[0]) != null;
        }

        private static IFubuRegistryExtension buildExtension(Type type)
        {
            var contextType = Type.GetType(type.AssemblyQualifiedName);
            return (IFubuRegistryExtension) Activator.CreateInstance(contextType);
        }
    }
}