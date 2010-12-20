using System.Collections.Generic;
using FubuMVC.Core.Packaging;

namespace Fubu.Packages
{
    public interface IPackageLogger
    {
        void WriteAssembliesNotFound(AssemblyFiles theAssemblyFiles, PackageManifest manifest, CreatePackageInput theInput);
    }
}