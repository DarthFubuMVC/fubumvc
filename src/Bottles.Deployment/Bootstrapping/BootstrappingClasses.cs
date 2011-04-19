using Bottles.Deployment.Runtime;
using StructureMap;
using StructureMap.Configuration.DSL;

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

    public class DeploymentRegistry : Registry
    {
        public DeploymentRegistry()
        {
            Scan(x =>
            {
                x.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.Contains("Deployers"));
                x.ConnectImplementationsToTypesClosing(typeof (IDeployer<>));
            });

            Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions();
            });
        }
    }
}