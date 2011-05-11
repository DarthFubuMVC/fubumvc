using Bottles.Deployers.Iis;
using Bottles.Deployment;
using Bottles.Deployment.Directives;
using Bottles.Deployment.Runtime.Content;
using Bottles.Diagnostics;
using Bottles.Exploding;
using Bottles.Tests.Deployment.Runtime;
using Bottles.Zipping;
using FubuCore;
using NUnit.Framework;

namespace Bottles.Tests.Deployment
{
    // TODO -- what is this accomplishing?
    [TestFixture]
    public class IntegrationIisFubuDeploymentTester
    {
        [Test][Explicit]
        public void DeployHelloWorld()
        {
            IFileSystem fileSystem = new FileSystem();
            var settings = new DeploymentSettings(@"C:\dev\test-profile\");
            IBottleRepository bottles = new BottleRepository(fileSystem, new ZipFileService(fileSystem), settings);

            var initializer = new IisFubuInitializer(fileSystem, new DeploymentSettings());
            
            var deployer = new IisFubuDeployer(fileSystem, bottles);
            

            var directive = new FubuWebsite();
            directive.WebsiteName = "fubu";
            directive.WebsitePhysicalPath = @"C:\dev\test-web";
            directive.VDir = "bob";
            directive.VDirPhysicalPath = @"C:\dev\test-app";
            directive.AppPool = "fubizzle";

            directive.DirectoryBrowsing = Activation.Enable;


            initializer.Execute(directive, new HostManifest("something"), new PackageLog());

            var host = new HostManifest("a");

            deployer.Execute(directive, host, new PackageLog());
        }
        
    }
}