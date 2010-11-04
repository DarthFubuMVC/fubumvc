using System;
using System.Collections.Generic;
using System.Reflection;

namespace FubuMVC.Core.Packaging
{
    public class PackageScanner
    {
        
        
        //public static readonly string PACKAGES_FOLDER = "fubu-packages";
        //public static readonly string PACKAGE_EXTENSION = "fubupak";

        public IEnumerable<PackageInfo> ScanForPackages()
        {
            throw new NotImplementedException();
        }

        public virtual PackageInfo ReadPackage(PackageManifest manifest)
        {
            throw new NotImplementedException();
        }

        public virtual PackageInfo ReadPackage(string folder, Func<Assembly, bool> filter)
        {
            throw new NotImplementedException();
        }
    }


    public interface IPackageInfo
    {
        IEnumerable<Assembly> Assemblies { get; }
    }


    // Bury file system stuff in here?  Kinda convention.  Can pull an interface out of it.
    public class PackageInfo : IPackageInfo
    {
        public string Name { get; set; }

        public IEnumerable<Assembly> Assemblies { get; set; }
        public string Folder { get; set; }
    }

    // Make this deal with the assembly resolve problem
    public static class PackageRegistry
    {
        public static IEnumerable<Assembly> ExtensionAssemblies
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        
    }

    public class PackageLoader
    {
        
    }

    public interface IPackageActivator
    {
        void Activate(IEnumerable<PackageInfo> packages);
    }
}