using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using System.Linq;

namespace FubuMVC.Core.Packaging
{
    /********
     * 1.) check for existing directory on the fly
     * 2.) method for deleting any package directories that aren't used
     *   a.) put in a bootstrapper
     *   b.) register in fubupackageregistry
     *   c.) test
     * 3.) load by package name, change ZipFilePackageReader
     * 
     */
    

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

        public IEnumerable<string> ExplodeAllZipsAndReturnPackageDirectories(string applicationDirectory)
        {
            var packageFileNames = findPackageFileNames(applicationDirectory);

            // Needs to be evaluated right now.
            return packageFileNames.Select(file => explodeZipAndReturnDirectory(file, applicationDirectory)).ToList();
        }

        private string explodeZipAndReturnDirectory(string file, string applicationDirectory)
        {
            var directory = directoryForZipFile(applicationDirectory, file);
            var request = new ExplodeRequest()
            {
                Directory = directory,
                ExplodeAction = () => Explode(applicationDirectory, file),
                GetVersion = () => _service.GetVersion(file),
                LogSameVersion = () => _logger.WritePackageZipFileWasSameVersionAsExploded(file)
            };


            explode(request);

            return directory;
        }

        

        

        private string directoryForZipFile(string applicationDirectory, string file)
        {
            return FileSystem.Combine(applicationDirectory, "bin", FubuMvcPackages.FubuPackagesFolder,
                                      Path.GetFileNameWithoutExtension(file));
        }

        public class ExplodeRequest
        {
            public Func<string> GetVersion { get; set;}
            public string Directory { get; set; }
            public Action ExplodeAction { get; set; }
            public Action LogSameVersion { get; set; }
        }

        private void explode(ExplodeRequest request)
        {

            if (_fileSystem.DirectoryExists(request.Directory))
            {
                var packageVersion = request.GetVersion();
                var folderVersion = ReadVersion(request.Directory);

                if (packageVersion == folderVersion)
                {
                    request.LogSameVersion();
                    return;
                }
            }

            request.ExplodeAction();
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

        public string ReadVersion(string directoryName)
        {
            var parts = new string[]{
                directoryName,
                FubuMvcPackages.VersionFile};

            // TODO -- harden?
            if (_fileSystem.FileExists(parts))
            {
                return _fileSystem.ReadStringFromFile(parts);
            }

            return Guid.Empty.ToString();
                
        }

        public void LogPackageState(string applicationDirectory)
        {
            //var existingDirectories = FindExplodedPackageDirectories(applicationDirectory);
            //var packageFileNames = findPackageFileNames(applicationDirectory);

            //_logger.WritePackageZipsFound(applicationDirectory, packageFileNames);
            //_logger.WriteExistingDirectories(applicationDirectory, existingDirectories);
        }


        public static string FolderForPackage(string name)
        {
            return Path.GetFileNameWithoutExtension(name);
        }
    }
}