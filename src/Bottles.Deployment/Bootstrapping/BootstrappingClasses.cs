using StructureMap.Configuration.DSL;

namespace Bottles.Deployment.Bootstrapping
{
    public class DeploymentBootstrapper
    {
        
    }

    public class DeploymentRegistry : Registry
    {
        public DeploymentRegistry()
        {
            Scan(x =>
            {
                x.AssembliesFromPath("deployers");
                x.ConnectImplementationsToTypesClosing(typeof (IDeployer<>));
            });
        }
    }
}