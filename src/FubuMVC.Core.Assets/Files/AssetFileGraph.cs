using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Assets.Files
{
    public class AssetFileGraph : IAssetFileRegistration, IAssetFileGraph
    {
        public static readonly string Application = "application";

        private readonly IList<PackageAssets> _allPackages = new List<PackageAssets>();
        private readonly Cache<string, PackageAssets> _packages;
        private readonly Cache<string, AssetFile> _memoizedFiles;

        public AssetFileGraph()
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
            if (path.Folder != null) file.Folder = path.Folder;
            _packages[path.Package].AddFile(path, file);
        }

        public IEnumerable<PackageAssets> AllPackages
        {
            get
            {
                return _allPackages;
            }
        }

        public IEnumerable<AssetFile> AllFiles()
        {
            return _allPackages.SelectMany(x => x.AllFiles());
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

        public AssetPath AssetPathOf(AssetFile file)
        {
            var package = _allPackages.First(x => x.AllFiles().Contains(file));
            return new AssetPath(package.PackageName, file.Name, file.Folder);
        }

        public AssetFile FindByPath(string path)
        {
            return _allPackages.SelectMany(x => x.FindByFilePath(path)).FirstOrDefault();
        }

        public PackageAssets AssetsFor(string packageName)
        {
            return _packages[packageName];
        }
    }
}