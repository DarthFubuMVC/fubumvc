using System.IO;
using System.Linq;
using Bottles.Deployment;
using Bottles.Deployment.Bootstrapping;
using Bottles.Deployment.Runtime.Content;
using Bottles.Deployment.Writing;
using Bottles.Diagnostics;
using Bottles.Exploding;
using Bottles.Zipping;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace Bottles.Tests.Deployment.Runtime.Content
{

    [TestFixture]
    public class when_handling_a_bottle_explosion_request_with_a_blank_bottle_repository : InteractionContext<BottleRepository>
    {
        private BottleExplosionRequest theRequest;

        protected override void beforeEach()
        {
            theRequest = new BottleExplosionRequest(new PackageLog()){
                BottleDirectory = string.Empty,
                BottleName = "bottle1",
                DestinationDirectory = "destination directory"
            };

            Services.Inject(new DeploymentSettings());
            Services.PartialMockTheClassUnderTest();

            

            ClassUnderTest.Expect(x => x.CopyTo("bottle1", "destination directory"));

            ClassUnderTest.ExplodeFiles(theRequest);
        }

        [Test]
        public void should_just_move_the_file_instead()
        {
            ClassUnderTest.VerifyAllExpectations();
        }
    }

    [TestFixture]
    public class BottleRepositoryTester : InteractionContext<BottleRepository>
    {
        private DeploymentSettings theSettings;
        private BottleRepository repo;

        protected override void beforeEach()
        {
            new DeploymentWriter(".\\brt").Flush(FlushOptions.Wipeout);
            theSettings = new DeploymentSettings(".\\brt");
            repo = new BottleRepository(MockFor<IFileSystem>(), MockFor<IZipFileService>(), theSettings);
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

            MockFor<IZipFileService>().AssertWasCalled(
                fs =>
                fs.ExtractTo(FileSystem.Combine(theSettings.BottlesDirectory, "bot.zip"), "x",
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