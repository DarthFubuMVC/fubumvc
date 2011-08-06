using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Assets.Files
{


    public enum AssetFileSource
    {
        Application,
        Package,
        External
    }

    
    //// Package stuff will be part of the AssetPipeline
    //public interface IAssetFile
    //{
    //    void ResetTimestamp(DateTime time);
    //    DateTime LastChanged { get; }

    //    string Url();
    //    string Name { get; set; }

    //    string ReadContents();

    //    string MimeType { get; }

    //    void ApplyTransformer(IAssetTransformer transformer);

    //}

    // Make AssetFile dumb
    public class AssetFile 
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public DateTime LastChanged { get; set; }
    }

    public enum AssetType
    {
        images,
        scripts,
        styles
    }

    //public class AssetPipeline
    //{
    //    private readonly PackageAssets
    //}

    // Forget CDN for now
    public class PackageAssets
    {
        private readonly Cache<AssetType, List<AssetFile>> _files = new Cache<AssetType, List<AssetFile>>(key => new List<AssetFile>());

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
    }


    //public interface IAssetTransformer
    //{
    //    bool Applies(AssetFile file);
    //    string Transform(string contents);

    //    string MimeType { get; }
    //}

    //public class AssetPipelineReader
    //{
    //    private readonly IFileSystem _system;

    //    public AssetPipelineReader(IFileSystem system)
    //    {
    //        _system = system;
    //    }

    //    public void ReadApplicationFolder(string directory, AssetPipeline pipeline)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void ReadPackageFolder(string directory, AssetPipeline pipeline)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    
}