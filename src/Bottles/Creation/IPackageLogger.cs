using Bottles.Assemblies;

namespace Bottles.Creation
{
    public interface IPackageLogger
    {
        void WriteAssembliesNotFound(AssemblyFiles theAssemblyFiles, PackageManifest manifest, CreatePackageInput theInput);
    }
}