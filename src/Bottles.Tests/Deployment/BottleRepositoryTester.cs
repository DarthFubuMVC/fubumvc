using Bottles.Deployment;
using Bottles.Deployment.Writing;
using Bottles.Exploding;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace Bottles.Tests.Deployment
{
    [TestFixture]
    public class BottleRepositoryTester : InteractionContext<BottleRepository>
    {
        private DeploymentSettings theSettings;
        BottleRepository repo;

        protected override void beforeEach()
        {
            new DeploymentWriter(".\\brt").Flush(FlushOptions.Wipeout);
            theSettings = new DeploymentSettings(".\\brt");
            repo = new BottleRepository(MockFor<IFileSystem>(), MockFor<IPackageExploder>(), theSettings);
        }


        [Test]
        public void ShouldCopyTo()
        {
            repo.CopyTo("bot","x");

            MockFor<IFileSystem>().AssertWasCalled(fs=>fs.Copy(FileSystem.Combine(theSettings.BottlesDirectory, "bot.zip"),"x"));
        }


        [Test]
        public void ShouldExlpodeTo()
        {
            repo.ExplodeTo("bot", "x");

            MockFor<IPackageExploder>().AssertWasCalled(fs => fs.Explode(FileSystem.Combine(theSettings.BottlesDirectory, "bot.zip"), "x", ExplodeOptions.PreserveDestination));
        }

        [Test, Ignore]
        public void ShouldExplodeFiles()
        {
            ClassUnderTest.ExplodeFiles(null);
        }


    }
}