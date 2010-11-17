using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using FubuCore;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Packaging
{
    // TODO -- maybe this gets renamed later, or moved into an application model / bootstrapper
    public static class PackageLoader
    {
        

        // This is here to redirect the application path in
        // testing scenarios
        public static string PhysicalRootPath { get; set; }
        private static IPackageActivator _assemblyResolverActivator = new AssemblyResolvePackageActivator();

        private static readonly List<PackageInfo> _packages = new List<PackageInfo>();
        private static readonly IList<IPackageActivator> _systemPackageActivators =
            new List<IPackageActivator>(){
                new VirtualPathProviderPackageActivator()
        };

        /// <summary>
        /// This should ONLY be used in testing scenarios
        /// </summary>
        public static IList<IPackageActivator> SystemPackageActivators
        {
            get { return _systemPackageActivators; }
        }

        public static IEnumerable<Assembly> ExtensionAssemblies
        {
            get
            {
                var assemblies = _packages.SelectMany(x => x.Assemblies).Distinct();
                return assemblies;
            }
        }

        public static void LoadPackages(Func<IEnumerable<IPackageActivator>> activators)
        {
            findAndResolvePackages();

            activators().Union(_systemPackageActivators).Each(x => x.Activate(_packages));
        }

        private static void findAndResolvePackages()
        {
            var applicationPath = PhysicalRootPath ?? HostingEnvironment.ApplicationPhysicalPath;

            LoadPackages(applicationPath);
        }

        public static void LoadPackages(string applicationPath)
        {
            _assemblyResolverActivator = new AssemblyResolvePackageActivator();

            _packages.Clear();
            var manifestLoader = new PackageManifestReader(applicationPath, new FileSystem());
            _packages.AddRange(manifestLoader.ReadAll());

            // Must set up the assembly resolve event before continuing
            // to build up the application
            _assemblyResolverActivator.Activate(_packages);
        }

        public static IEnumerable<IFubuRegistryExtension> FindAllExtensions()
        {
            if (!ExtensionAssemblies.Any()) return new IFubuRegistryExtension[0];

            var pool = new TypePool(null){
                ShouldScanAssemblies = true
            };
            pool.AddAssemblies(ExtensionAssemblies);

            // Yeah, it really does have to be this way
            return pool.TypesMatching(t => hasDefaultCtor(t) && t.GetInterfaces().Any(i => i.FullName == typeof(IFubuRegistryExtension).FullName) )
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