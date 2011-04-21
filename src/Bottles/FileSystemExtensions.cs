using FubuCore;

namespace Bottles
{
    public static class FileSystemExtensions
    {
        public static bool PackageManifestExists(this IFileSystem fileSystem, string folder)
        {
            return fileSystem.FileExists(folder, PackageManifest.FILE);
        }

        public static PackageManifest LoadPackageManifestFrom(this IFileSystem fileSystem, string folder)
        {
            return fileSystem.LoadFromFile<PackageManifest>(folder, PackageManifest.FILE);
        }
    }
}