using Bottles.Deployment;
using Bottles.Deployment.Bootstrapping;
using Bottles.Deployment.Deployers;
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
        public void bob()
        {
            var writer = new ProfileWriter(@"C:\Users\dsellers.fcs\Desktop\ProfileScratch\x", new FileSystem());
            writer.AddEnvironmentSetting("name","dru");

            var r = writer.RecipeFor("FubuTestApplication");
            
            var h = r.HostFor("web");

            var d = new IisFubuWebsite();

            d.WebsiteName = "test";
            d.WebsitePhysicalPath = @"C:\dev\test-web";
            d.VDir = "bob";
            d.VDirPhysicalPath = @"C:\dev\test-app";
            d.HostBottle = "FubuTestApplication";
            d.AppPool = "fubu-test";
            
            h.AddDirective(d);
            
            writer.Flush();




            var settings = new DeploymentSettings(@"C:\Users\dsellers.fcs\Desktop\ProfileScratch\x\");
            var container = DeploymentBootstrapper.Bootstrap(settings);
            var deploymentController = container.GetInstance<IDeploymentController>();
            deploymentController.Deploy();
        }

    }
}