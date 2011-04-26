using Bottles.Exploding;
using FubuCore;

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
            var path = GetPathForBottle(bottleName);
            _fileSystem.Copy(path, destination);
        }

        public void ExplodeTo(string bottleName, string destination)
        {
            var path = GetPathForBottle(bottleName);

            //REVIEW: get_app_dir, zip-filename == path???
            _exploder.Explode(PackageRegistry.GetApplicationDirectory(), path, destination, ExplodeOptions.PreserveDestination);
        }

        string GetPathForBottle(string bottleName)
        {
            if (!bottleName.EndsWith(BottleFiles.Extension))
                bottleName = bottleName + "." + BottleFiles.Extension;

            //this should be a file
            return FileSystem.Combine(_settings.BottlesDirectory, bottleName);
        }
    }
}