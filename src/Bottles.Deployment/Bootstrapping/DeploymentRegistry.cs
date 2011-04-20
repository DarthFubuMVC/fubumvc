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
                x.ConnectImplementationsToTypesClosing(typeof (IInitializer<>));
                x.ConnectImplementationsToTypesClosing(typeof (IDeployer<>));
                x.ConnectImplementationsToTypesClosing(typeof (IFinalizer<>));
            });

            Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions();
            });
        }
    }
}