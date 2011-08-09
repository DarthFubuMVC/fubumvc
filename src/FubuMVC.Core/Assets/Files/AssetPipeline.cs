using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Assets.Files
{
    public interface IAssetFileRegistration
    {
        void AddFile(AssetPath path, AssetFile file);
    }

    public class AssetPipeline : IAssetFileRegistration
    {
        public static readonly string Application = "application";

        private readonly IList<PackageAssets> _allPackages = new List<PackageAssets>();
        private readonly Cache<string, PackageAssets> _packages;
        private readonly Cache<string, AssetFile> _memoizedFiles;

        public AssetPipeline()
        {
            _packages = new Cache<string, PackageAssets>(name =>
            {
                var package = new PackageAssets(name);
                _allPackages.Add(package);

                return package;
            });

            _packages.FillDefault(Application);

            _memoizedFiles = new Cache<string, AssetFile>(path => Find(new AssetPath(path)));
        }

        public void AddFile(AssetPath path, AssetFile file)
        {
            _packages[path.Package].AddFile(path, file);               
        }

        public IEnumerable<PackageAssets> AllPackages
        {
            get
            {
                return _allPackages;
            }
        }

        // Not worrying about throwing exceptions for something not found here.
        public AssetFile Find(AssetPath path)
        {
            if (path.Package.IsNotEmpty())
            {
                return _packages[path.Package].FindByPath(path).FirstOrDefault();
            }

            var files = _allPackages.SelectMany(x => x.FindByPath(path));

            return files.FirstOrDefault(x => x.Override) ?? files.FirstOrDefault();
        }

        public AssetFile Find(string path)
        {
            return _memoizedFiles[path];
        }

        public PackageAssets AssetsFor(string packageName)
        {
            return _packages[packageName];
        }
    }
}