using System;
using FubuCore;
using NUnit.Framework;
using FubuTestingSupport;

namespace Bottles.Tests
{
    [TestFixture]
    public class DeploymentFolderFinderTester
    {
        private DeploymentFolderFinder theFinder;
        private FileSystem fileSystem;

        [SetUp]
        public void BeforeEach()
        {
            fileSystem = new FileSystem();
            theFinder = new DeploymentFolderFinder(fileSystem);
        }

        private void putDeploymentFolderHere(string location)
        {
            fileSystem.CreateDirectory(location);
            fileSystem.WriteStringToFile(FileSystem.Combine(location, ProfileFiles.BottlesManifestFile), "");
        }

        [Test]
        public void looks_deployment_folder_at_the_exact_spot()
        {
            fileSystem.DeleteDirectory(".\\dep1");
            putDeploymentFolderHere(".\\dep1");
            theFinder.FindDeploymentFolder(".\\dep1").ShouldEqual(".\\dep1");
        }

        [Test]
        public void looks_deployment_folder_at_one_level_deeper()
        {
            fileSystem.DeleteDirectory(".\\dep2");
            putDeploymentFolderHere(".\\dep2\\deployment");
            theFinder.FindDeploymentFolder(".\\dep2").ShouldEqual(".\\dep2\\deployment");
        }

        [Test]
        public void could_find_path_should_throw()
        {
            Assert.Throws<Exception>(() =>
            {
                theFinder.FindDeploymentFolder(".\\dep3");
            });

        }
    }
}