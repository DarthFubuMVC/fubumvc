using Bottles.Deployers.Iis;
using Bottles.Deployment;
using Bottles.Deployment.Directives;
using Bottles.Exploding;
using Bottles.Tests.Deployment.Runtime;
using Bottles.Zipping;
using FubuCore;
using NUnit.Framework;

namespace Bottles.Tests.Deployment
{
    [TestFixture]
    public class IntegrationIisFubuDeploymentTester
    {
        [Test][Explicit]
        public void DeployHelloWorld()
        {
            IFileSystem fileSystem = new FileSystem();
            var settings = new DeploymentSettings(@"C:\dev\test-profile\");
            IBottleRepository bottles = new BottleRepository(fileSystem, new PackageExploder(new ZipFileService(fileSystem), new PackageExploderLogger(s=>{ }), fileSystem ), settings);

            var fakeDeploymentDiagnostics = new FakeDeploymentDiagnostics();

            var initializer = new IisFubuInitializer(fileSystem, fakeDeploymentDiagnostics, new DeploymentSettings());
            
            var deployer = new IisFubuDeployer(fileSystem, bottles, fakeDeploymentDiagnostics);
            

            var directive = new FubuWebsite();
            directive.HostBottle = "test";
            directive.WebsiteName = "fubu";
            directive.WebsitePhysicalPath = @"C:\dev\test-web";
            directive.VDir = "bob";
            directive.VDirPhysicalPath = @"C:\dev\test-app";
            directive.AppPool = "fubizzle";

            directive.DirectoryBrowsing = Activation.Enable;


            initializer.Initialize(directive);

            var host = new HostManifest("a");

            deployer.Deploy(host, directive);
        }
        
    }
}