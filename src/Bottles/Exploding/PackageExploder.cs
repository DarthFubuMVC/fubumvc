using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bottles.Zipping;
using FubuCore;

namespace Bottles.Exploding
{
    public enum ExplodeOptions
    {
        DeleteDestination,
        PreserveDestination
    }

    public class PackageExploder : IPackageExploder
    {
        // TODO -- better logging?
        public static PackageExploder GetPackageExploder(FileSystem fileSystem)
        {
            return new PackageExploder(new ZipFileService(), new PackageExploderLogger(x => Console.WriteLine(x)), fileSystem);
        }

        private readonly IFileSystem _fileSystem;
        private readonly IPackageExploderLogger _logger;
        private readonly IZipFileService _service;

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

        //destinationDirectory = var directoryName = BottleFiles.DirectoryForPackageZipFile(applicationDirectory, sourceZipFile);
        public void Explode(string applicationDirectory, string sourceZipFile, string destinationDirectory, ExplodeOptions options)
        {
            if(options == ExplodeOptions.DeleteDestination)
                _fileSystem.DeleteDirectory(destinationDirectory);

            _logger.WritePackageZipFileExploded(sourceZipFile, destinationDirectory);
            _service.ExtractTo(sourceZipFile, destinationDirectory);
        }

        public void CleanAll(string applicationDirectory)
        {
            var directory = BottleFiles.GetExplodedPackagesDirectory(applicationDirectory);
            clearExplodedDirectories(directory);

            // This is here for legacy installations that may have old exploded packages in bin/fubu-packages
            clearExplodedDirectories(BottleFiles.GetApplicationPackagesDirectory(applicationDirectory));
        }

        private void clearExplodedDirectories(string directory)
        {
            _fileSystem.ChildDirectoriesFor(directory).Each(x =>
            {
                _logger.WritePackageDirectoryDeleted(x);
                _fileSystem.DeleteDirectory(x);
            });
        }

        public string ReadVersion(string directoryName)
        {
            var parts = new[]{
                directoryName,
                BottleFiles.VersionFile
            };

            // TODO -- harden?
            if (_fileSystem.FileExists(parts))
            {
                return _fileSystem.ReadStringFromFile(parts);
            }

            return Guid.Empty.ToString();
        }

        public void LogPackageState(string applicationDirectory)
        {
            // TODO -- log assemblies too
            logExplodedDirectories(applicationDirectory);
            logZipFiles(applicationDirectory);
        }

        private string explodeZipAndReturnDirectory(string file, string applicationDirectory)
        {
            var directory = BottleFiles.DirectoryForPackageZipFile(applicationDirectory, file);
            var request = new ExplodeRequest{
                Directory = directory,
                ExplodeAction = () => Explode(applicationDirectory, file, BottleFiles.DirectoryForPackageZipFile(applicationDirectory, file), ExplodeOptions.DeleteDestination),
                GetVersion = () => _service.GetVersion(file),
                LogSameVersion = () => _logger.WritePackageZipFileWasSameVersionAsExploded(file)
            };


            explode(request);

            return directory;
        }

        public void ExplodeAssembly(string applicationDirectory, Assembly assembly, IPackageFiles files)
        {
            var directory = BottleFiles.GetDirectoryForExplodedPackage(applicationDirectory, assembly.GetName().Name);

            var request = new ExplodeRequest{
                Directory = directory,
                GetVersion = () => assembly.GetName().Version.ToString(),
                LogSameVersion =
                    () =>
                    Console.WriteLine(
                        "Assembly {0} has already been 'exploded' onto disk".ToFormat(assembly.GetName().FullName)),
                ExplodeAction = () => explodeAssembly(assembly, directory)
            };

            explode(request);

            _fileSystem.ChildDirectoriesFor(directory).Each(child =>
            {
                var name = Path.GetFileName(child);

                files.RegisterFolder(name, child.ToFullPath());
            });
        }

        private void explodeAssembly(Assembly assembly, string directory)
        {
            _fileSystem.DeleteDirectory(directory);
            _fileSystem.CreateDirectory(directory);

            assembly.GetManifestResourceNames().Where(BottleFiles.IsEmbeddedPackageZipFile).Each(name =>
            {
                var folderName = BottleFiles.EmbeddedPackageFolderName(name);
                var stream = assembly.GetManifestResourceStream(name);

                var description = "Resource {0} in Assembly {1}".ToFormat(name, assembly.GetName().FullName);
                var destinationFolder = FileSystem.Combine(directory, folderName);

                _service.ExtractTo(description, stream, destinationFolder);

                var version = assembly.GetName().Version.ToString();
                _fileSystem.WriteStringToFile(FileSystem.Combine(directory, BottleFiles.VersionFile), version);
            });
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
            var fileSet = new FileSet{
                Include = "*.zip"
            };

            var packageFolder = BottleFiles.GetApplicationPackagesDirectory(applicationDirectory);

            return _fileSystem.FileNamesFor(fileSet, packageFolder);
        }

        private void logZipFiles(string applicationDirectory)
        {
            var packageFileNames = findPackageFileNames(applicationDirectory);
            _logger.WritePackageZipsFound(applicationDirectory, packageFileNames);
        }

        private void logExplodedDirectories(string applicationDirectory)
        {
            var explodedDirectory = BottleFiles.GetExplodedPackagesDirectory(applicationDirectory);
            var existingDirectories = _fileSystem.ChildDirectoriesFor(explodedDirectory);
            _logger.WriteExistingDirectories(applicationDirectory, existingDirectories);
        }




        #region Nested type: ExplodeRequest

        public class ExplodeRequest
        {
            public Func<string> GetVersion { get; set; }
            public string Directory { get; set; }
            public Action ExplodeAction { get; set; }
            public Action LogSameVersion { get; set; }
        }

        #endregion
    }

    
}