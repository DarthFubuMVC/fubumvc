using System;
using System.IO;
using Bottles.Creation;
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

        public static string FindBinaryDirectory(this IFileSystem fileSystem, string folder, CompileTargetEnum target)
        {
            var binFolder = FileSystem.Combine(folder, "bin");
            var debugFolder = FileSystem.Combine(binFolder, target.ToString());
            if (fileSystem.DirectoryExists(debugFolder))
            {
                binFolder = debugFolder;
            }

            return binFolder;
        }
    }
}