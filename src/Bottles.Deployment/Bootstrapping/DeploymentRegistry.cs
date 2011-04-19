using StructureMap.Configuration.DSL;

namespace Bottles.Deployment.Bootstrapping
{
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