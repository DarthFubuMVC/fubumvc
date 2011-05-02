using System.Linq;
using Bottles.Diagnostics;
using FubuCore;
using NUnit.Framework;
using FubuTestingSupport;

namespace Bottles.Tests
{
    [TestFixture]
    public class SolutionDirectoryPackageLoaderTester
    {
        private string thePathToScan = ".\\solDirPackLoad";
        private SolutionDirectoryPackageLoader theLoader;

        [SetUp]
        public void BeforeEach()
        {
            var fs = new FileSystem();
            fs.DeleteDirectory(thePathToScan);
            fs.CreateDirectory(thePathToScan);
            fs.CreateDirectory(thePathToScan, "bin");

            theLoader = new SolutionDirectoryPackageLoader(thePathToScan.ToFullPath());
            var manifest = new PackageManifest();
            manifest.Name = "test-mani";

            fs.PersistToFile(manifest, thePathToScan, PackageManifest.FILE);
        }

        [Test]
        public void there_are_7_manifests_that_are_modules_in_fubu()
        {
            var foundPackages = theLoader.Load(new PackageLog());
            foundPackages.Count().ShouldEqual(1);
            foundPackages.First().Name.ShouldEqual("test-mani");
        }
    }
}