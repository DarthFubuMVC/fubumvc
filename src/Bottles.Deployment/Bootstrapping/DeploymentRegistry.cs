using System;
using Bottles.Deployment.Deployers;
using Bottles.Exploding;
using Bottles.Zipping;
using StructureMap.Configuration.DSL;

namespace Bottles.Deployment.Bootstrapping
{
    public class DeploymentRegistry : Registry
    {
        public DeploymentRegistry()
        {
            Scan(x =>
            {
                //REVIEW: what does this map to? it may not be 'profile/delpoyers'
                x.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.Contains("Deployers"));

                //REVIEW: Ugly?
                x.AssemblyContainingType<IisFubuWebsite>();

                x.ConnectImplementationsToTypesClosing(typeof (IInitializer<>));
                x.ConnectImplementationsToTypesClosing(typeof (IDeployer<>));
                x.ConnectImplementationsToTypesClosing(typeof (IFinalizer<>));
            });

            For<IPackageExploder>().Use<PackageExploder>();
            For<IZipFileService>().Use<ZipFileService>();
            For<IPackageExploderLogger>().Use(new PackageExploderLogger(Console.WriteLine));

            Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions();
            });
        }
    }
}