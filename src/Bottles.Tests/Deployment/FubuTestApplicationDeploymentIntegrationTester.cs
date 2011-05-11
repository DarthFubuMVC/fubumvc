using System.IO;
using Bottles.Deployment;
using Bottles.Deployment.Bootstrapping;
using Bottles.Deployment.Directives;
using Bottles.Deployment.Runtime;
using Bottles.Deployment.Writing;
using FubuCore;
using NUnit.Framework;

namespace Bottles.Tests.Deployment
{
    [TestFixture]
    public class FubuTestApplicationDeploymentIntegrationTester
    {
        [Test][Explicit]
        public void DeployWebsite()
        {
            var testRoot = Path.GetFullPath(@".\integration");

            var writer = new DeploymentWriter(testRoot, new FileSystem());
            writer.AddEnvironmentSetting("name","dru");
            
            
            var r = writer.RecipeFor("FubuTestApplication");
            
            var h = r.HostFor("web");

            var d = new Website();

            d.WebsiteName = "test";
            d.WebsitePhysicalPath = @"C:\dev\test-web";
            d.VDir = "bob";
            d.VDirPhysicalPath = @"C:\dev\test-app";
            d.AppPool = "fubu-test";
            
            h.AddDirective(d);
            
            writer.Flush(FlushOptions.Wipeout);

            //copy over bottles
            var fileName = "FubuTestApplication.zip";
            var destination = @".\integration\bottles\";
            Directory.CreateDirectory(destination);
            File.Copy(@"C:\Users\dsellers.fcs\Desktop\ProfileScratch\FubuTestApplication.zip", Path.Combine(destination, fileName), true);
            //<stop>


            var settings = new DeploymentSettings(testRoot)
                           {
                               UserForced = true
                           };

            var container = DeploymentBootstrapper.Bootstrap(settings);
            var deploymentController = container.GetInstance<IDeploymentController>();
            deploymentController.Deploy(new DeploymentOptions());
        }

    }
}