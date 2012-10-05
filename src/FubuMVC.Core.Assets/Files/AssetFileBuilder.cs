using System.IO;
using FubuCore;

namespace FubuMVC.Core.Assets.Files
{
    public class AssetFileBuilder
    {
        private readonly AssetFolder _assetFolder;
        private readonly PackageAssetDirectory _directory;
        private readonly IAssetFileRegistration _registration;
        private readonly string _specificDirectory;

        public AssetFileBuilder(IAssetFileRegistration registration, string specificDirectory,
                                PackageAssetDirectory directory, AssetFolder assetFolder)
        {
            _registration = registration;
            _specificDirectory = specificDirectory;
            _directory = directory;
            _assetFolder = assetFolder;
        }

        public void CreateAssetFile(string filename)
        {
            var name = filename.PathRelativeTo(_specificDirectory).Replace(Path.DirectorySeparatorChar, '/');
            var path = new AssetPath(_directory.PackageName, name, _assetFolder);
            var file = new AssetFile(name){
                FullPath = filename.ToFullPath()
            };

            _registration.AddFile(path, file);
        }
    }
}