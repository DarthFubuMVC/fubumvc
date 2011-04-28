using System.IO;
using System.Linq;
using Bottles.Deployment;
using Bottles.Deployment.Bootstrapping;
using Bottles.Deployment.Writing;
using Bottles.Diagnostics;
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
        private BottleRepository repo;

        protected override void beforeEach()
        {
            new DeploymentWriter(".\\brt").Flush(FlushOptions.Wipeout);
            theSettings = new DeploymentSettings(".\\brt");
            repo = new BottleRepository(MockFor<IFileSystem>(), MockFor<IPackageExploder>(), theSettings);
        }


        [Test]
        public void ShouldCopyTo()
        {
            repo.CopyTo("bot", "x");

            MockFor<IFileSystem>().AssertWasCalled(
                fs => fs.Copy(FileSystem.Combine(theSettings.BottlesDirectory, "bot.zip"), "x"));
        }


        [Test]
        public void ShouldExlpodeTo()
        {
            repo.ExplodeTo("bot", "x");

            MockFor<IPackageExploder>().AssertWasCalled(
                fs =>
                fs.Explode(FileSystem.Combine(theSettings.BottlesDirectory, "bot.zip"), "x",
                           ExplodeOptions.PreserveDestination));
        }

        [Test, Ignore]
        public void ShouldExplodeFiles()
        {
            ClassUnderTest.ExplodeFiles(null);
        }
    }

    [TestFixture]
    public class BottleRepositoryIntegratedTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var settings = new DeploymentSettings(FileSystem.Combine("TestData", ProfileFiles.DeploymentFolder));
            var container = DeploymentBootstrapper.Bootstrap(settings);
            theRepository = container.GetInstance<IBottleRepository>();
        }

        #endregion

        private IBottleRepository theRepository;

        [Test]
        public void explode_files()
        {
            new FileSystem().DeleteDirectory("exploded");

            var packageLog = new PackageLog();
            theRepository.ExplodeFiles(new BottleExplosionRequest(packageLog){
                BottleDirectory = "Config",
                BottleName = "Fake",
                DestinationDirectory = FileSystem.Combine("exploded", "config")
            });

            packageLog.FullTraceText().ShouldContain("1.config");
            packageLog.FullTraceText().ShouldContain("2.config");
            packageLog.FullTraceText().ShouldContain("3.config");
            packageLog.FullTraceText().ShouldContain(FileSystem.Combine("a", "1.config"));
            packageLog.FullTraceText().ShouldContain(FileSystem.Combine("a", "2.config"));

            new FileSystem().FindFiles(FileSystem.Combine("exploded", "Config"), new FileSet{
                DeepSearch = true,
                Include = "*.*"
            }).Select(x => Path.GetFileName(x)).ShouldHaveTheSameElementsAs("1.config", "2.config", "3.config",
                                                                            "1.config", "2.config");
        }
    }
}