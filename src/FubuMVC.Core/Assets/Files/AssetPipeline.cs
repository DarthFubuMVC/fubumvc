using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Assets.Files
{
    public class AssetPipeline
    {
        
    }

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

    // Forget CDN for now
    public class PackageAssets
    {
        private readonly Cache<AssetType, List<AssetFile>> _files = new Cache<AssetType, List<AssetFile>>(key => new List<AssetFile>());


        public void AddFile(AssetType type, AssetFile file)
        {
            _files[type].Add(file);
        }

        public AssetFile FindByName(string name)
        {
            throw new NotImplementedException();
        }

        public AssetFile FindByPath(string path)
        {
            throw new NotImplementedException();
        }


    }


    //public interface IAssetTransformer
    //{
    //    bool Applies(AssetFile file);
    //    string Transform(string contents);

    //    string MimeType { get; }
    //}

    public class AssetPipelineReader
    {
        private readonly IFileSystem _system;

        public AssetPipelineReader(IFileSystem system)
        {
            _system = system;
        }

        public void ReadApplicationFolder(string directory, AssetPipeline pipeline)
        {
            throw new NotImplementedException();
        }

        public void ReadPackageFolder(string directory, AssetPipeline pipeline)
        {
            throw new NotImplementedException();
        }
    }

    
}