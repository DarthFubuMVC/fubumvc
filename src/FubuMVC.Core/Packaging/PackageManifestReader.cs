using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
    public class PackageManifestReader : IPackageLoader, IPackageManifestReader
    {
        private readonly string _applicationFolder;
        private readonly IFileSystem _fileSystem;
        private readonly Func<string, string> _getContentFolderFromPackageFolder;

        public PackageManifestReader(string applicationFolder, IFileSystem fileSystem, Func<string, string> getContentFolderFromPackageFolder)
        {
            if (!Path.IsPathRooted(applicationFolder))
            {
                throw new ArgumentOutOfRangeException("applicationFolder", "Only absolute paths can be used here");
            }

            _applicationFolder = applicationFolder;
            _fileSystem = fileSystem;
            _getContentFolderFromPackageFolder = getContentFolderFromPackageFolder;
        }

        public IEnumerable<IPackageInfo> Load()
        {
            var includes = _fileSystem.LoadFromFile<ApplicationManifest>(_applicationFolder, ApplicationManifest.FILE);

            return includes.LinkedFolders.Select(f => LoadFromFolder(Path.Combine(_applicationFolder, f)));
        }

        public IPackageInfo LoadFromFolder(string folder)
        {
            folder = Path.GetFullPath(folder);

            var manifest = _fileSystem.LoadFromFile<PackageManifest>(folder, PackageManifest.FILE);
            var package = new PackageInfo(manifest.Name){
                Description = "{0} ({1})".ToFormat(manifest.Name, folder)
            };


            // Right here, this needs to be different
            package.RegisterFolder(FubuMvcPackages.WebContentFolder, _getContentFolderFromPackageFolder(folder));
            package.RegisterFolder(PackageInfo.DataFolder, Path.Combine(folder, PackageInfo.DataFolder));

            var binPath = FileSystem.Combine(_applicationFolder, folder, "bin");


            var assemblyPaths = findCandidateAssemblyFiles(binPath);
            assemblyPaths.Each(path =>
            {
                var assemblyName = Path.GetFileNameWithoutExtension(path);
                if (manifest.AssemblyNames.Contains(assemblyName))
                {
                    package.RegisterAssemblyLocation(assemblyName, path);
                }
            });

            return package;
        }

        private static IEnumerable<string> findCandidateAssemblyFiles(string binPath)
        {
            return Directory.GetFiles(binPath).Where(IsPotentiallyAnAssembly);
        }

        public static bool IsPotentiallyAnAssembly(string file)
        {
            var extension = Path.GetExtension(file);
            return extension.Equals(".exe", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".dll", StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return "Package Manifest Reader (Development Mode)";
        }
    }
}