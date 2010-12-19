using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using Ionic.Zip;

namespace Fubu.Packages
{
    public class ZipFileWrapper : IZipFile
    {
        private readonly ZipFile _file;

        public ZipFileWrapper(ZipFile file)
        {
            _file = file;
        }

        public void AddFile(string fileName)
        {
            _file.AddFile(fileName);
        }

        public void AddFile(string fileName, string zipFolder)
        {
            _file.AddFile(fileName, zipFolder);
        }

        // TODO -- need to test this
        public Guid AddVersion()
        {
            throw new NotImplementedException();
        }

        public void AddFiles(ZipFolderRequest request)
        {
            request.WriteToZipFile(this);
        }
    }


    public class ZipFolderRequest
    {
        public FileSet FileSet { get; set; }
        public string RootDirectory { get; set; }
        public string ZipDirectory { get; set; }

        // TODO -- need to unit test this little beastie
        public void WriteToZipFile(IZipFile zipFile)
        {
            var cache = new Cache<string, string>(file => Path.Combine(ZipDirectory, file.PathRelativeTo(RootDirectory)));

            FileSet.IncludedFilesFor(RootDirectory).Each(cache.FillDefault);
            FileSet.ExcludedFilesFor(RootDirectory).Each(cache.Remove);

            cache.Each((file, name) => zipFile.AddFile(file, Path.GetDirectoryName(name)));
        }
    }



    public class PackageCreator
    {
        private readonly IFileSystem _fileSystem;
        private readonly IZipFileCreator _zipFileCreator;
        private readonly IPackageLogger _logger;

        public PackageCreator(IFileSystem fileSystem, IZipFileCreator zipFileCreator, IPackageLogger logger)
        {
            _fileSystem = fileSystem;
            _zipFileCreator = zipFileCreator;
            _logger = logger;
        }

        public void CreatePackage(CreatePackageInput input)
        {
            var manifest = _fileSystem.LoadPackageManifestFrom(input.PackageFolder);

            _zipFileCreator.CreateZipFile(input.ZipFile, zipFile =>
            {
                zipFile.AddFile(_fileSystem.PackageManifestPathFor(input.PackageFolder), "");


                if (!TryFindAllAssemblies(manifest, input, zipFile))
                {
                    return;
                }

                AddDataFiles(input, zipFile, manifest);

                AddContentFiles(input, zipFile, manifest);
            });
        }

        public void AddContentFiles(CreatePackageInput input, IZipFile zipFile, PackageManifest manifest)
        {
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

        public bool TryFindAllAssemblies(PackageManifest manifest, CreatePackageInput input, IZipFile zipFile)
        {
            var binFolder = FileSystem.Combine(input.PackageFolder, "bin");

            var candidates = Directory.GetFiles(binFolder)
                .Where(x => PackageManifestReader.IsPotentiallyAnAssembly(x) && manifest.AssemblyNames.Contains(Path.GetFileNameWithoutExtension(x)));

            if (candidates.Count() == manifest.AssemblyNames.Count())
            {
                candidates.Each(file => zipFile.AddFile(file, "bin"));
                return true;
            }

            // TODO -- needs to throw!!!
            _logger.WriteAssembliesNotFound(input, manifest, candidates);
            return false;
        }
    }
}