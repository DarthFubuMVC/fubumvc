using System;
using Bottles.Exploding;
using Bottles.Zipping;
using FubuCore;
using FubuCore.Binding;
using StructureMap;

namespace Bottles.Deployment.Bootstrapping
{
    public static class DeploymentBootstrapper
    {
        public static IContainer Bootstrap(DeploymentSettings settings)
        {
            return new Container(x =>
            {
                // TODO -- might come back to this and make it smarter
                x.For<IObjectResolver>().Use(ObjectResolver.Basic());

                x.For<IFileSystem>().Use<FileSystem>();

                x.For<IProfileFinder>().Use<DeploymentFolderFinder>();
                x.For<IPackageExploder>().Use<PackageExploder>();
                x.For<IZipFileService>().Use<ZipFileService>();
                x.For<IPackageExploderLogger>().Use(new PackageExploderLogger(Console.WriteLine));

                x.For<DeploymentSettings>().Use(settings);

                x.AddRegistry<DeploymentRegistry>();
                
            });
        }
    }
}