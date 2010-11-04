using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Hosting;
using FubuCore;
using System.Linq;

namespace FubuMVC.Core.Packaging
{

    // Bury file system stuff in here?  Kinda convention.  
    public class PackageInfo
    {
        public string Name { get; set; }

        public IEnumerable<Assembly> Assemblies { get; set; }
        public string FilesFolder { get; set; }
    }

    // Make this deal with the assembly resolve problem
    // Add the virtual path provider crap here
    public static class PackageRegistry
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

    public interface IPackageActivator
    {
        void Activate(IEnumerable<PackageInfo> packages);
    }

    public class VirtualPathProviderPackageActivator : IPackageActivator
    {
        public void Activate(IEnumerable<PackageInfo> packages)
        {
            var provider = new FileSystemVirtualPathProvider();
            HostingEnvironment.RegisterVirtualPathProvider(provider);

            packages.Each(x => provider.RegisterContentDirectory(x.FilesFolder));
        }
    }

    public class AssemblyResolvePackageActivator : IPackageActivator
    {
        public void Activate(IEnumerable<PackageInfo> packages)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (s, args) => PackageRegistry.ExtensionAssemblies.FirstOrDefault(assembly =>
            {
                return args.Name == assembly.GetName().Name || args.Name == assembly.GetName().FullName;
            });
        }
    }
}