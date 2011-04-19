using StructureMap;

namespace Bottles.Deployment.Bootstrapping
{
    public static class DeploymentBootstrapper
    {
        private static readonly IContainer _container = new Container(new DeploymentRegistry());

        public static IContainer Container
        {
            get { return _container; }
        }
    }
}