using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuMVC.Core.Assets.Files
{
    public class PackageAssets
    {
        private readonly Cache<AssetType, List<AssetFile>> _files = new Cache<AssetType, List<AssetFile>>(key => new List<AssetFile>());

        public PackageAssets(string packageName)
        {
            PackageName = packageName;
        }

        public string PackageName { get; private set;}

        public void AddFile(AssetPath path, AssetFile file)
        {
            if (!path.Type.HasValue)
            {
                throw new ArgumentException("AssetPath must have an AssetType to be used here");
            }

            _files[path.Type.Value].Add(file);
        }

        public AssetFile FindByName(string name)
        {
            var path = new AssetPath(name);
            return FindByPath(path).SingleOrDefault();
        }

        public IEnumerable<AssetFile> FindByPath(AssetPath path)
        {
            if (path.Type.HasValue)
            {
                return matchingType(path.Type.Value, path.Name);
            }

            var scripts = matchingType(AssetType.scripts, path.Name);
            if (scripts.Any()) return scripts;

            var styles = matchingType(AssetType.styles, path.Name);
            if (styles.Any()) return styles;

            var images = matchingType(AssetType.images, path.Name);
            if (images.Any()) return images;

            return new AssetFile[0];
        }

        private IEnumerable<AssetFile> matchingType(AssetType type, string name)
        {
            return _files[type].Where(x => x.Name == name);
        }

        public IEnumerable<AssetFile> FilesForAssetType(AssetType type)
        {
            return _files[type];
        }
    }
}