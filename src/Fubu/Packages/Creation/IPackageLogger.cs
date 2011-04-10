using Bottles.Assemblies;
using FubuMVC.Core.Packaging;

namespace Fubu.Packages.Creation
{
    public interface IPackageLogger
    {
        void WriteAssembliesNotFound(AssemblyFiles theAssemblyFiles, PackageManifest manifest, CreatePackageInput theInput);
    }
}