using System;
using System.IO;
using Bottles.Exploding;
using Bottles.Zipping;
using FubuCore;
using System.Collections.Generic;

namespace Bottles.Deployment
{
    public class BottleRepository : IBottleRepository
    {
        private readonly IFileSystem _fileSystem;
        private readonly IZipFileService _zipService;
        private readonly DeploymentSettings _settings;

        public BottleRepository(IFileSystem fileSystem, IZipFileService zipService, DeploymentSettings settings)
        {
            _fileSystem = fileSystem;
            _zipService = zipService;
            _settings = settings;
        }

        public void CopyTo(string bottleName, string destination)
        {
            var path = pathForBottle(bottleName);
            _fileSystem.Copy(path, destination);
        }

        public void ExplodeTo(string bottleName, string destination)
        {
            var bottleFile = pathForBottle(bottleName);

            // TODO -- needs logging?
            //REVIEW: get_app_dir, zip-filename == path???
            _zipService.ExtractTo(bottleFile, destination, ExplodeOptions.PreserveDestination);
        }

        public void ExplodeFiles(BottleExplosionRequest request)
        {
            var bottleName = request.BottleName;

            var bottleFile = pathForBottle(bottleName);
            _fileSystem.CreateDirectory(_settings.StagingDirectory);

            var tempDirectory = FileSystem.Combine(_settings.StagingDirectory, bottleName);


            explodeToStaging(request, bottleFile, tempDirectory);

            var sourceDirectory = FileSystem.Combine(tempDirectory, request.BottleDirectory);

            _fileSystem.CreateDirectory(request.DestinationDirectory);

            _fileSystem.FindFiles(sourceDirectory, new FileSet(){
                DeepSearch = true,
                Include = "*.*"
            }).Each(file =>
            {
                var destinationFile = FileSystem.Combine(request.DestinationDirectory, file.PathRelativeTo(sourceDirectory));
                request.Log.Trace("Copying {0} to {1}", file, destinationFile);
                
                _fileSystem.Copy(file, destinationFile);
            });
        }

        public PackageManifest ReadManifest(string bottleName)
        {
            throw new NotImplementedException();
        }

        private readonly IList<string> _bottlesExplodedToStaging = new List<string>();
        private void explodeToStaging(BottleExplosionRequest request, string bottleFile, string tempDirectory)
        {
            if (_bottlesExplodedToStaging.Contains(request.BottleName))
            {
                return;
            }

            request.Log.Trace("Exploding bottle {0} to {1}");
            _zipService.ExtractTo(bottleFile, tempDirectory, ExplodeOptions.DeleteDestination);
        
            _bottlesExplodedToStaging.Add(request.BottleName);
        }

        string pathForBottle(string bottleName)
        {
            if (!bottleName.EndsWith(BottleFiles.Extension))
                bottleName = bottleName + "." + BottleFiles.Extension;

            //this should be a file
            return FileSystem.Combine(_settings.BottlesDirectory, bottleName);
        }
    }
}