using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuMVC.Core;
using System.Linq;

namespace Fubu.Packages
{

    public interface IPackageExploderLogger
    {
        void WritePackageDirectoryDeleted(string directoryName);
        void WritePackageZipFileExploded(string zipFile, string directoryName);
        void WritePackageZipFileWasSameVersionAsExploded(string file);
    }

    public class PackageExploder
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
            var fileSet = new FileSet(){
                Include = "*.zip"
            };

            var existingDirectories = _fileSystem.ChildDirectoriesFor(applicationDirectory, "bin",
                                                                      FubuMvcPackages.FubuPackagesFolder);

            _fileSystem.FileNamesFor(fileSet, applicationDirectory, "bin", FubuMvcPackages.FubuPackagesFolder).Each(file =>
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


        public static string FolderForPackage(string name)
        {
            return Path.GetFileNameWithoutExtension(name);
        }
    }
}