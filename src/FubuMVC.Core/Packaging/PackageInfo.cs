using System.Collections.Generic;
using System.Reflection;

namespace FubuMVC.Core.Packaging
{

    // Bury file system stuff in here?  Kinda convention.  
    public interface IPackageInfo
    {
        string Name { get; }
        void LoadAssemblies(IAssemblyLoader loader);
    }

    public class PackageInfo
    {
        public string Name { get; set; }

        public IEnumerable<Assembly> Assemblies { get; set; }
        public string FilesFolder { get; set; }
    }
}