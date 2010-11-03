using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using System.Linq;

namespace FubuMVC.Core.Packaging
{

    [XmlType("package")]
    public class PackageManifest
    {
        public static readonly string FILE = "fubu-package-manifest.xml";

        private readonly IList<string> _assemblyNames = new List<string>();

        public string Name { get; set; }

        public string Assemblies 
        {
            get
            {
                return _assemblyNames.Join(";");
            }
            set
            {
                var names = value.Split(';').Select(x => x.Trim());
                _assemblyNames.Clear();
                _assemblyNames.AddRange(names);
            }
        }

        public IEnumerable<string> AssemblyNames
        {
            get
            {
                return _assemblyNames;
            }
        }
    }

    public class PackageScanner
    {
        
        public static readonly string PACKAGES_FOLDER = "fubu-packages";
        public static readonly string PACKAGE_EXTENSION = "fubupak";

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