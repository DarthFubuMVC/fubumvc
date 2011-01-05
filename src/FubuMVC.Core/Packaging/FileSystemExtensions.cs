using FubuCore;

namespace FubuMVC.Core.Packaging
{
    public static class FileSystemExtensions
    {
        public static bool ApplicationManifestExists(this IFileSystem fileSystem, string appFolder)
        {
            return fileSystem.FileExists(appFolder, ApplicationManifest.FILE);
        }

        public static bool PackageManifestExists(this IFileSystem fileSystem, string folder)
        {
            return fileSystem.FileExists(folder, PackageManifest.FILE);
        }

        public static PackageManifest LoadPackageManifestFrom(this IFileSystem fileSystem, string folder)
        {
            return fileSystem.LoadFromFile<PackageManifest>(folder, PackageManifest.FILE);
        }

        public static ApplicationManifest LoadApplicationManifestFrom(this IFileSystem fileSystem, string folder)
        {
            return fileSystem.LoadFromFile<ApplicationManifest>(folder, ApplicationManifest.FILE);
        }

        public static string ApplicationManifestPathFor(this IFileSystem fileSystem, string folder)
        {
            return FileSystem.Combine(folder, ApplicationManifest.FILE);
        }
    }
}