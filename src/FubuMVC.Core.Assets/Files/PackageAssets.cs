using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Assets.Files
{
    public class PackageAssets
    {
        private readonly Cache<AssetFolder, List<AssetFile>> _files = new Cache<AssetFolder, List<AssetFile>>(key => new List<AssetFile>());

        public PackageAssets(string packageName)
        {
            PackageName = packageName;
        }

        public string PackageName { get; private set;}

        public void AddFile(AssetPath path, AssetFile file)
        {
            if (path.Folder == null)
            {
                throw new ArgumentException("AssetPath must have an AssetType to be used here");
            }

            _files[path.Folder].Add(file);
        }

        public AssetFile FindByName(string name)
        {
            var path = new AssetPath(name);
            return FindByPath(path).SingleOrDefault();
        }

        public IEnumerable<AssetFile> FindByPath(AssetPath path)
        {
            if (path.Folder != null)
            {
                return matchingType(path.Folder, path.Name);
            }

            var scripts = matchingType(AssetFolder.scripts, path.Name);
            if (scripts.Any()) return scripts;

            var styles = matchingType(AssetFolder.styles, path.Name);
            if (styles.Any()) return styles;

            var images = matchingType(AssetFolder.images, path.Name);
            if (images.Any()) return images;

            return new AssetFile[0];
        }

        private IEnumerable<AssetFile> matchingType(AssetFolder folder, string name)
        {
            return _files[folder].Where(x => x.Name.EqualsIgnoreCase(name));           
        }

        public IEnumerable<AssetFile> FilesForAssetType(AssetFolder folder)
        {
            return _files[folder];
        }

        public IEnumerable<AssetFile> AllFiles()
        {
            return _files.GetAll().SelectMany(x => x);
        }

        public IEnumerable<AssetFile> FindByFilePath(string path)
        {
            return AllFiles().Where(x => x.MatchesFullPath(path));
        }
    }
}