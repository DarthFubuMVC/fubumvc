using Bottles.Deployment;
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
            var settings = new DeploymentSettings("dir");

            settings.BottlesDirectory.ShouldEqual("dir\\bottles");
            settings.RecipesDirectory.ShouldEqual("dir\\recipes");
            settings.EnvironmentFile.ShouldEqual("dir\\environment.settings");

            settings.TargetDirectory.ShouldEqual("dir\\target");
        }
    }
}