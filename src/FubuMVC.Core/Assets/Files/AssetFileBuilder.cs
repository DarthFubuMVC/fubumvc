using System.IO;
using FubuCore;

namespace FubuMVC.Core.Assets.Files
{
    public class AssetFileBuilder
    {
        private readonly IAssetFileRegistration _registration;
        private readonly string _specificDirectory;
        private readonly PackageAssetDirectory _directory;
        private readonly AssetType _assetType;

        public AssetFileBuilder(IAssetFileRegistration registration, string specificDirectory, PackageAssetDirectory directory, AssetType assetType)
        {
            _registration = registration;
            _specificDirectory = specificDirectory;
            _directory = directory;
            _assetType = assetType;
        }

        public void CreateAssetFile(string filename)
        {
            var name = filename.PathRelativeTo(_specificDirectory).Replace(Path.DirectorySeparatorChar, '/');
            var path = new AssetPath(_directory.PackageName, name, _assetType);
            var file = new AssetFile()
                       {
                           FullPath = filename.ToFullPath(),
                           Name = name
                       };

            _registration.AddFile(path, file);
        }
    }
}