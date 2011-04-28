using System;
using System.IO;
using Bottles.Exploding;
using FubuCore;
using System.Collections.Generic;

namespace Bottles.Deployment
{
    public class BottleRepository : IBottleRepository
    {
        private readonly IFileSystem _fileSystem;
        private readonly IPackageExploder _exploder;
        private readonly DeploymentSettings _settings;

        public BottleRepository(IFileSystem fileSystem, IPackageExploder exploder, DeploymentSettings settings)
        {
            _fileSystem = fileSystem;
            _settings = settings;
            _exploder = exploder;
        }

        public void CopyTo(string bottleName, string destination)
        {
            var path = pathForBottle(bottleName);
            _fileSystem.Copy(path, destination);
        }

        public void ExplodeTo(string bottleName, string destination)
        {
            var bottleFile = pathForBottle(bottleName);

            //REVIEW: get_app_dir, zip-filename == path???
            _exploder.Explode(bottleFile, destination, ExplodeOptions.PreserveDestination);
        }

        public void ExplodeFiles(BottleExplosionRequest request)
        {
            var bottleFile = pathForBottle(request.BottleName);
            
            _fileSystem.CreateDirectory(_settings.StagingDirectory);

            var tempDirectory = FileSystem.Combine(_settings.StagingDirectory, request.BottleName);

            request.Log.Trace("Exploding bottle {0} to {1}");
            _exploder.Explode(bottleFile, tempDirectory, ExplodeOptions.DeleteDestination);

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

        string pathForBottle(string bottleName)
        {
            if (!bottleName.EndsWith(BottleFiles.Extension))
                bottleName = bottleName + "." + BottleFiles.Extension;

            //this should be a file
            return FileSystem.Combine(_settings.BottlesDirectory, bottleName);
        }
    }
}