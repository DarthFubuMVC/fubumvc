using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
    // TODO -- maybe this gets renamed later, or moved into an application model / bootstrapper
    public static class PackageLoader
    {
        private static readonly List<PackageInfo> _packages = new List<PackageInfo>();
        private static readonly IEnumerable<IPackageActivator> _systemPackages = 
            new IPackageActivator[]{new VirtualPathProviderPackageActivator(), new AssemblyResolvePackageActivator()};

        public static IEnumerable<Assembly> ExtensionAssemblies
        {
            get
            {
                return _packages.SelectMany(x => x.Assemblies).Distinct();
            }
        }

        public static void LoadPackages(Func<IEnumerable<IPackageActivator>> activators)
        {
            _packages.Clear();

            var applicationPath = HostingEnvironment.ApplicationPhysicalPath;
            var manifestLoader = new PackageManifestReader(applicationPath, new FileSystem());
            _packages.AddRange(manifestLoader.ReadAll());

            activators().Union(_systemPackages).Each(x => x.Activate(_packages));
        }   
    }
}