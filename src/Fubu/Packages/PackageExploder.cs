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
        void WritePackageZipsFound(string applicationDirectory, IEnumerable<string> packageFileNames);
        void WriteExistingDirectories(string applicationDirectory, IEnumerable<string> existingDirectories);
    }

    public class PackageExploderLogger : IPackageExploderLogger
    {
        private readonly Action<string> _writer;

        public PackageExploderLogger(Action<string> writer)
        {
            _writer = writer;
        }

        private void write(string format, params object[] parameters)
        {
            _writer(format.ToFormat(parameters));
        }

        public void WritePackageDirectoryDeleted(string directoryName)
        {
            write("Deleted exploded package directory {0}", directoryName);
        }

        public void WritePackageZipFileExploded(string zipFile, string directoryName)
        {
            write("Exploded package zip file {0} to {1}", zipFile, directoryName);
        }

        public void WritePackageZipFileWasSameVersionAsExploded(string file)
        {
            write("Current version of package file {0} is already exploded to the application folder", file);
        }

        public void WritePackageZipsFound(string applicationDirectory, IEnumerable<string> packageFileNames)
        {
            if (packageFileNames.Any())
            {
                Console.WriteLine("Found these package zip files:");
                packageFileNames.Each(x => Console.WriteLine("  " + x));
            }
            else
            {
                Console.WriteLine("No package zip files found for the application at {0}", applicationDirectory);
            }
        }


        public void WriteExistingDirectories(string applicationDirectory, IEnumerable<string> existingDirectories)
        {
            if (existingDirectories.Any())
            {
                Console.WriteLine("Found {0} exploded package directories in the application at {0}", applicationDirectory);

                existingDirectories.Each(dir => Console.WriteLine("  " + dir));
            }
            else
            {
                Console.WriteLine("No exploded package directories in the application at {0}", applicationDirectory);
            }
        }
    }

    public interface IPackageExploder
    {
        void ExplodeAll(string applicationDirectory);
        void Explode(string applicationDirectory, string zipFile);
        void CleanAll(string applicationDirectory);
        Guid ReadVersion(string directoryName);
        void LogPackageState(string applicationDirectory);
    }

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
            var existingDirectories = findExistingPackageDirectories(applicationDirectory);
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

        private IEnumerable<string> findExistingPackageDirectories(string applicationDirectory)
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
            var existingDirectories = findExistingPackageDirectories(applicationDirectory);
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