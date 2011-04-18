using System;
using Bottles.Exploding;
using FubuCore;

namespace Bottles.Deployment
{
    public class BottleRepository : IBottleRepository
    {
        private readonly IFileSystem _fileSystem;
        private readonly IProfile _root;
        private IPackageExploder _exploder;

        public BottleRepository(IFileSystem fileSystem, IProfile root, IPackageExploder exploder)
        {
            _fileSystem = fileSystem;
            _exploder = exploder;
            _root = root;
        }

        public void CopyTo(string bottleName, string destination)
        {
            var path = _root.GetPathForBottle(bottleName);
            _fileSystem.Copy(path, destination);
        }

        public void ExplodeTo(string bottleName, string destination)
        {
            var path = _root.GetPathForBottle(bottleName);

            //REVIEW: get_app_dir, zip-filename == path???
            _exploder.Explode(PackageRegistry.GetApplicationDirectory(), path, destination, ExplodeOptions.PreserveDestination);
        }
    }
}