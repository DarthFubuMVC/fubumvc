using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Assets.Diagnostics;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Packaging;

namespace FubuMVC.Core.Assets
{
    public class AssetFileGraphBuilderActivator : IActivator
    {
        private readonly IAssetFileRegistration _pipeline;

        public AssetFileGraphBuilderActivator(IAssetFileRegistration pipeline, AssetLogsCache assetLogsCache)
        {
            _pipeline = new RecordingAssetFileRegistrator(pipeline, assetLogsCache);
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            var builder = new AssetFileGraphBuilder(new FileSystem(), _pipeline, log);
            findDirectories(packages).Each(builder.LoadFiles);
        }

        private static IEnumerable<PackageAssetDirectory> findDirectories(IEnumerable<IPackageInfo> packages)
        {
            yield return new PackageAssetDirectory(){
                Directory = FubuMvcPackageFacility.GetApplicationPath(),
                PackageName = AssetFileGraph.Application
            };

            foreach (var pak in packages)
            {
                string directory = null;
                pak.ForFolder(BottleFiles.WebContentFolder, dir =>
                {
                    directory = dir; 
                });

                if (directory.IsNotEmpty())
                {
                    yield return new PackageAssetDirectory(){
                        Directory = directory,
                        PackageName = pak.Name
                    };
                }
            }
        }

        public override string ToString()
        {
            return "Building the assetFileGraph from the application and package content folders";
        }
    }

    public class RecordingAssetFileRegistrator : IAssetFileRegistration
    {
        private readonly IAssetFileRegistration _inner;
        private readonly AssetLogsCache _assetLogsCache;

        public RecordingAssetFileRegistrator(IAssetFileRegistration inner, AssetLogsCache assetLogsCache)
        {
            _inner = inner;
            _assetLogsCache = assetLogsCache;
        }

        public void AddFile(AssetPath path, AssetFile file)
        {
            _assetLogsCache.FindByName(file.Name).Add(path.Package,"Adding {0} to IAssetFileGraph".ToFormat(file.FullPath));
            _inner.AddFile(path, file);
        }
    }
}