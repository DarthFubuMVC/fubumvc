using FubuCore;
using StructureMap;

namespace Bottles.Deployment.Bootstrapping
{
    public static class DeploymentBootstrapper
    {
        public static IContainer Bootstrap(DeploymentSettings settings)
        {
            return new Container(x =>
            {
                x.For<IFileSystem>().Use<FileSystem>();

                x.AddRegistry<DeploymentRegistry>();
                x.For<DeploymentSettings>().Use(settings);
            });
        }
    }
}