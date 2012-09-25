using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
    public interface IPackageService
    {
        /// <summary>
        /// Removes the exploded contents of all packages
        /// </summary>
        /// <param name="applicationDirectory"></param>
        void CleanAllPackages(string applicationDirectory);

        /// <summary>
        /// Removes all package files and exploded package contents from an application
        /// </summary>
        /// <param name="applicationDirectory"></param>
        void RemoveAllPackages(string applicationDirectory);

        void InstallPackage(string applicationFolder, string packageFileName, bool uninstallFlag);
    }

    public class PackageService : IPackageService
    {
        private readonly IFileSystem _fileSystem;

        public PackageService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        private static IEnumerable<string> findPackageFolders(string applicationDirectory)
        {
            FubuMvcPackageFacility.PhysicalRootPath = applicationDirectory;
            return FubuMvcPackageFacility.GetPackageDirectories();
        }

        public void InstallPackage(string applicationFolder, string packageFileName, bool uninstallFlag)
        {
            var packageFolder = FileSystem.Combine(applicationFolder, "bin", FubuMvcPackageFacility.FubuPackagesFolder);

            var destinationFileName = FileSystem.Combine(packageFolder, Path.GetFileName(packageFileName));

            if (uninstallFlag)
            {
                if (_fileSystem.FileExists(destinationFileName))
                {
                    Console.WriteLine("Deleting existing file " + destinationFileName);
                    _fileSystem.DeleteFile(destinationFileName);
                }
                else
                {
                    Console.WriteLine("File {0} does not exist", destinationFileName);
                }


                return;
            }

            if (!Directory.Exists(packageFolder))
            {
                Console.WriteLine("Creating folder " + packageFolder);
                _fileSystem.CreateDirectory(packageFolder);
            }


            if (File.Exists(destinationFileName))
            {
                Console.WriteLine("Deleting existing file at " + destinationFileName);
                _fileSystem.DeleteFile(destinationFileName);
            }

            Console.WriteLine("Copying {0} to {1}", packageFileName, packageFolder);

            _fileSystem.Copy(packageFileName, destinationFileName);
        }

        /// <summary>
        /// Removes the exploded contents of all packages
        /// </summary>
        /// <param name="applicationDirectory"></param>
        public void CleanAllPackages(string applicationDirectory)
        {
            findPackageFolders(applicationDirectory).Each(dir =>
            {
                Console.WriteLine();
                _fileSystem.ChildDirectoriesFor(dir).Each(x =>
                {
                    Console.WriteLine("Deleting directory " + x);
                    _fileSystem.DeleteDirectory(x);
                });
            });
        }

        /// <summary>
        /// Removes all package files and exploded package contents from an application
        /// </summary>
        /// <param name="applicationDirectory"></param>
        public void RemoveAllPackages(string applicationDirectory)
        {
            findPackageFolders(applicationDirectory).Each(dir =>
            {
                Console.WriteLine("Cleaning all the contents of directory " + dir);
                _fileSystem.CleanDirectory(dir);
            });
        }
    }
}