using System.IO;
using System.Linq;
using FubuCore;

namespace Bottles
{
    public static class BottleFiles
    {
        static BottleFiles()
        {
            ContentFolder = "content";
            PackagesFolder = "packages";
        }

        public static readonly string Extension = "zip";

        public static readonly string WebContentFolder = "WebContent";
        public static readonly string VersionFile = ".version";
        public static readonly string DataFolder = "data";
        public static readonly string ConfigFolder = "config";
        public static readonly string BinaryFolder = "bin";

        public static readonly string BottlesFolder = "bottles";

        public static string ContentFolder { get; set; }
        public static string PackagesFolder { get; set; }

        public static readonly FileSet DataFiles = FileSet.Deep("data/*");
        public static readonly FileSet ConfigFiles = FileSet.Deep("config/*");


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
            var dir = applicationDirectory.AppendPath(ContentFolder, packageName);
            return dir.ToFullPath();
        }

        public static string GetApplicationPackagesDirectory(string applicationDirectory)
        {
            var dir = applicationDirectory.AppendPath(PackagesFolder);
            return dir.ToFullPath();
        }

        public static string GetExplodedPackagesDirectory(string applicationDirectory)
        {
            return applicationDirectory.AppendPath(ContentFolder).ToFullPath();
        }
    }
}