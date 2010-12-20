using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using System.Linq;

namespace FubuMVC.Core.Packaging
{
    public class PackageExploder : IPackageExploder
    {
        private readonly IZipFileService _service;
        private readonly IPackageExploderLogger _logger;
        private readonly IFileSystem _fileSystem;

        public PackageExploder(IZipFileService service, IPackageExploderLogger logger, IFileSystem fileSystem)
        {
            _service = service;
            _logger = logger;
            _fileSystem = fileSystem;
        }

        public void ExplodeAll(string applicationDirectory)
        {
            var existingDirectories = FindExplodedPackageDirectories(applicationDirectory);
            var packageFileNames = findPackageFileNames(applicationDirectory);
            packageFileNames.Each(file =>
            {
                var directoryName = FileSystem.Combine(applicationDirectory, "bin", FubuMvcPackages.FubuPackagesFolder,
                                                       Path.GetFileNameWithoutExtension(file));

                if (existingDirectories.Contains(directoryName))
                {
                    var zipVersion = _service.GetVersion(file);
                    var folderVersion = ReadVersion(directoryName);

                    if (zipVersion == folderVersion)
                    {
                        _logger.WritePackageZipFileWasSameVersionAsExploded(file);
                        return;
                    }
                }
                
                
                Explode(applicationDirectory, file);
                
            });
        }

        public IEnumerable<string> FindExplodedPackageDirectories(string applicationDirectory)
        {
            return _fileSystem.ChildDirectoriesFor(applicationDirectory, "bin",
                                                   FubuMvcPackages.FubuPackagesFolder);
        }

        private IEnumerable<string> findPackageFileNames(string applicationDirectory)
        {
            var fileSet = new FileSet()
                          {
                              Include = "*.zip"
                          };
            return _fileSystem.FileNamesFor(fileSet, applicationDirectory, "bin", FubuMvcPackages.FubuPackagesFolder);
        }

        public void Explode(string applicationDirectory, string zipFile)
        {
            var directoryName = FileSystem.Combine(applicationDirectory, "bin", FubuMvcPackages.FubuPackagesFolder, Path.GetFileNameWithoutExtension(zipFile));
            _fileSystem.DeleteDirectory(directoryName);

            _logger.WritePackageZipFileExploded(zipFile, directoryName);
            _service.ExtractTo(zipFile, directoryName);
        }

        public void CleanAll(string applicationDirectory)
        {
            
            _fileSystem.ChildDirectoriesFor(applicationDirectory, "bin", FubuMvcPackages.FubuPackagesFolder).Each(x =>
            {
                _logger.WritePackageDirectoryDeleted(x);
                _fileSystem.DeleteDirectory(x);
            });

        }

        public Guid ReadVersion(string directoryName)
        {
            var parts = new string[]{
                directoryName,
                FubuMvcPackages.VersionFile};

            // TODO -- harden?
            if (_fileSystem.FileExists(parts))
            {
                var raw = _fileSystem.ReadStringFromFile(parts);
                return new Guid(raw);
            }

            return Guid.Empty;
                
        }

        public void LogPackageState(string applicationDirectory)
        {
            var existingDirectories = FindExplodedPackageDirectories(applicationDirectory);
            var packageFileNames = findPackageFileNames(applicationDirectory);

            _logger.WritePackageZipsFound(applicationDirectory, packageFileNames);
            _logger.WriteExistingDirectories(applicationDirectory, existingDirectories);
        }


        public static string FolderForPackage(string name)
        {
            return Path.GetFileNameWithoutExtension(name);
        }
    }
}