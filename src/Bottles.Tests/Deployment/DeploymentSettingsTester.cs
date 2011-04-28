using Bottles.Deployment;
using Bottles.Deployment.Writing;
using NUnit.Framework;
using FubuTestingSupport;

namespace Bottles.Tests.Deployment
{
    [TestFixture]
    public class DeploymentSettingsTester
    {
        [Test]
        public void build_the_default_ctor()
        {
            setupValidDeploymentFolderAt("dir");

            //review there is a check inside of here
            var settings = new DeploymentSettings("dir");

            settings.BottlesDirectory.ShouldEqual("dir\\bottles");
            settings.RecipesDirectory.ShouldEqual("dir\\recipes");
            settings.EnvironmentFile.ShouldEqual("dir\\environment.settings");
            settings.TargetDirectory.ShouldEqual("dir\\target");
            settings.DeploymentDirectory.ShouldEqual("dir");
            settings.BottleManifestFile.ShouldEqual("dir\\bottles.manifest");


            settings.GetHost("x", "z").ShouldEqual("dir\\recipes\\x\\z.host");
            settings.GetRecipe("a").ShouldEqual("dir\\recipes\\a");
        }

        private void setupValidDeploymentFolderAt(string name)
        {
            var pr = new DeploymentWriter(name);
            pr.Flush(FlushOptions.Wipeout);
        }
    }
}