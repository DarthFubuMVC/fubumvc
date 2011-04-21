using Bottles;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
    public static class FileSystemExtensions
    {
        public static bool ApplicationManifestExists(this IFileSystem fileSystem, string appFolder)
        {
            return fileSystem.FileExists(appFolder, ApplicationManifest.APPLICATION_MANIFEST_FILE);
        }

        public static ApplicationManifest LoadApplicationManifestFrom(this IFileSystem fileSystem, string folder)
        {
            return fileSystem.LoadFromFile<ApplicationManifest>(folder, ApplicationManifest.APPLICATION_MANIFEST_FILE);
        }

        public static string ApplicationManifestPathFor(this IFileSystem fileSystem, string folder)
        {
            return FileSystem.Combine(folder, ApplicationManifest.APPLICATION_MANIFEST_FILE);
        }
    }
}