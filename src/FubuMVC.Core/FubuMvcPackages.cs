using System.IO;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core
{
    public static class FubuMvcPackages
    {
        public static readonly string WebContentFolder = "WebContent";
        public static readonly string VersionFile = ".version";
        public static readonly string FubuPackagesFolder = "fubu-packages";
        public static readonly string DataFolder = "Data";
        public static readonly string FubuContentFolder = "fubu-content";



        public static string FolderForPackage(string name)
        {
            return Path.GetFileNameWithoutExtension(name);
        }

        public static bool IsEmbeddedPackageZipFile(string resourceName)
        {
            var parts = resourceName.Split('.');

            if (parts.Length < 2) return false;
            if (parts.Last().ToLower() != "zip") return false;
            return parts[parts.Length - 2].ToLower().StartsWith("pak");
        }

        public static string EmbeddedPackageFolderName(string resourceName)
        {
            var parts = resourceName.Split('.');
            return parts[parts.Length - 2].Replace("pak-", "");
        }

        public static string DirectoryForPackageZipFile(string applicationDirectory, string file)
        {
            var packageName = Path.GetFileNameWithoutExtension(file);
            return GetDirectoryForExplodedPackage(applicationDirectory, packageName);
        }

        public static string GetDirectoryForExplodedPackage(string applicationDirectory, string packageName)
        {
            return FileSystem.Combine(applicationDirectory, FubuContentFolder, packageName);
        }

        public static string GetApplicationPackagesDirectory(string applicationDirectory)
        {
            return FileSystem.Combine(applicationDirectory, "bin", FubuMvcPackages.FubuPackagesFolder);
        }

        public static string GetExplodedPackagesDirectory(string applicationDirectory)
        {
            return FileSystem.Combine(applicationDirectory, FubuContentFolder);
        }
    }
}