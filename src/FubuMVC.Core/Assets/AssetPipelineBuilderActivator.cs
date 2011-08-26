using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets
{
    public class AssetPipelineBuilderActivator : IActivator
    {
        private readonly IAssetFileRegistration _pipeline;

        public AssetPipelineBuilderActivator(IAssetFileRegistration pipeline)
        {
            _pipeline = pipeline;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            var builder = new AssetPipelineBuilder(new FileSystem(), _pipeline, log);
            findDirectories(packages).Each(builder.LoadFiles);
        }

        private static IEnumerable<PackageAssetDirectory> findDirectories(IEnumerable<IPackageInfo> packages)
        {
            yield return new PackageAssetDirectory(){
                Directory = ".".ToFullPath(),
                PackageName = AssetPipeline.Application
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
            return "Building the AssetPipeline from the application and package content folders";
        }
    }
}