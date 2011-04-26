using Bottles;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
    public static class FileSystemExtensions
    {
        public static bool ApplicationManifestExists(this IFileSystem fileSystem, string appFolder)
        {
            return fileSystem.FileExists(appFolder, PackageManifest.APPLICATION_MANIFEST_FILE);
        }

        public static PackageManifest LoadApplicationManifestFrom(this IFileSystem fileSystem, string folder)
        {
            return fileSystem.LoadFromFile<PackageManifest>(folder, PackageManifest.APPLICATION_MANIFEST_FILE);
        }

        public static string ApplicationManifestPathFor(this IFileSystem fileSystem, string folder)
        {
            return FileSystem.Combine(folder, PackageManifest.APPLICATION_MANIFEST_FILE);
        }
    }
}