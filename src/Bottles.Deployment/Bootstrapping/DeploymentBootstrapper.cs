using StructureMap;

namespace Bottles.Deployment.Bootstrapping
{
    public static class DeploymentBootstrapper
    {
        public static IContainer Bootstrap(DeploymentSettings settings)
        {
            return new Container(x =>
            {
                x.AddRegistry<DeploymentRegistry>();
                x.For<DeploymentSettings>().Use(settings);
            });
        }
    }
}