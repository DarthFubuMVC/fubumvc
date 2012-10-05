using System;
using Bottles.Diagnostics;
using FubuCore;
using System.Collections.Generic;

namespace FubuMVC.Core.Assets.Files
{
    public class PackageAssetDirectory
    {
        public string PackageName { get; set; }
        public string Directory { get; set; }
    }

    public class AssetFileGraphBuilder
    {
        public static readonly string NoContentFoundForPackageAt = "No content folders found underneath {0}";
        public static readonly string LoadingContentForPackageAt = "Discovered asset files at {0}";
        public static readonly string LoadingAssetTypeForPackageAt = "  loading from /{0}";

        private readonly IFileSystem _system;
        private readonly IAssetFileRegistration _registration;
        private readonly IPackageLog _log;

        public AssetFileGraphBuilder(IFileSystem system, IAssetFileRegistration registration, IPackageLog log)
        {
            _system = system;
            _registration = registration;
            _log = log;
        }

        // TODO -- gotta put some serious logging here when we get to diagnostics
        // and package loading
        public void LoadFiles(PackageAssetDirectory directory)
        {
            var contentFolder = FindContentFolder(directory.Directory);
            if (contentFolder == null)
            {
                string theMessage = NoContentFoundForPackageAt.ToFormat(directory.Directory);
                _log.Trace(theMessage);

                return;
            }

            _log.Trace(LoadingContentForPackageAt.ToFormat(contentFolder));
            LoadFilesFromContentFolder(directory, contentFolder);
        }

        public virtual void LoadFilesFromContentFolder(PackageAssetDirectory directory, string contentFolder)
        {
            ReadAssetType(directory, contentFolder, AssetFolder.styles);
            

            ReadAssetType(directory, contentFolder, AssetFolder.images);
            ReadAssetType(directory, contentFolder, AssetFolder.scripts);
        }

        public void ReadAssetType(PackageAssetDirectory directory, string contentFolder, AssetFolder assetFolder)
        {
            var specificFolder = contentFolder.AppendPath(assetFolder.ToString());
            if (_system.DirectoryExists(specificFolder))
            {
                _log.Trace(LoadingAssetTypeForPackageAt.ToFormat(specificFolder));
                var builder = new AssetFileBuilder(_registration, specificFolder, directory, assetFolder);

                _system.FindFiles(specificFolder, FileSet.Everything()).Each(builder.CreateAssetFile);
            }
        }


        public virtual string FindContentFolder(string directory)
        {
            // Look for "content" first, then "Content"
            var lowerCase = directory.AppendPath("content");
            if (_system.DirectoryExists(lowerCase)) return lowerCase;

            var upperCase = directory.AppendPath("Content");
            if (_system.DirectoryExists(upperCase)) return upperCase;

            return null;
        }
    }
}