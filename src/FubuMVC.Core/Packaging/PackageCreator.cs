using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
    public class PackageCreator
    {
        private readonly IFileSystem _fileSystem;
        private readonly IZipFileService _zipFileService;
        private readonly IPackageLogger _logger;
        private readonly IAssemblyFileFinder _assemblyFinder;

        public PackageCreator(IFileSystem fileSystem, IZipFileService zipFileService, IPackageLogger logger, IAssemblyFileFinder assemblyFinder)
        {
            _fileSystem = fileSystem;
            _zipFileService = zipFileService;
            _logger = logger;
            _assemblyFinder = assemblyFinder;
        }

        public void CreatePackage(CreatePackageInput input, PackageManifest manifest)
        {
            var binFolder = Path.Combine(input.PackageFolder, "bin");
            var assemblies = _assemblyFinder.FindAssemblies(binFolder, manifest.AssemblyNames);
            if (assemblies.Success)
            {
                writeZipFile(input, manifest, assemblies);
            }
            else
            {
                _logger.WriteAssembliesNotFound(assemblies, manifest, input);
            }
        }

        private void writeZipFile(CreatePackageInput input, PackageManifest manifest, AssemblyFiles assemblies)
        {
            _zipFileService.CreateZipFile(input.ZipFile, zipFile =>
            {
                assemblies.Files.Each(file =>
                {
                    zipFile.AddFile(file, "bin");
                });

                if (input.PdbFlag)
                {
                    assemblies.PdbFiles.Each(file =>
                    {
                        zipFile.AddFile(file, "bin");
                    });
                }

                WriteVersion(zipFile);

                zipFile.AddFile(FileSystem.Combine(input.PackageFolder, PackageManifest.FILE), "");

                AddDataFiles(input, zipFile, manifest);

                AddContentFiles(input, zipFile, manifest);
            });
        }

        public Guid WriteVersion(IZipFile zipFile)
        {
            var versionFile = Path.Combine(Path.GetTempPath(), FubuMvcPackages.VersionFile);
            var guid = Guid.NewGuid();
            _fileSystem.WriteStringToFile(versionFile, guid.ToString());
            zipFile.AddFile(versionFile);

            return guid;
        }

        public void AddContentFiles(CreatePackageInput input, IZipFile zipFile, PackageManifest manifest)
        {
            manifest.ContentFileSet.AppendExclude(FileSystem.Combine("bin","*.*"));

            zipFile.AddFiles(new ZipFolderRequest()
                             {
                                 FileSet = manifest.ContentFileSet,
                                 ZipDirectory = FubuMvcPackages.WebContentFolder,
                                 RootDirectory = input.PackageFolder
                             });
        }

        public void AddDataFiles(CreatePackageInput input, IZipFile zipFile, PackageManifest manifest)
        {
            zipFile.AddFiles(new ZipFolderRequest()
                             {
                                 FileSet = manifest.DataFileSet,
                                 ZipDirectory = PackageInfo.DataFolder,
                                 RootDirectory = Path.Combine(input.PackageFolder, PackageInfo.DataFolder)
                             });
        }

    }
}