using System.IO;
using FubuMVC.Core.Packaging;
using NUnit.Framework;
using FubuCore;
using System.Linq;

namespace FubuMVC.Tests.Packaging
{
    [TestFixture]
    public class PackageManifestReaderIntegratedTester
    {
        private string packageFolder;
        private PackageManifestReader reader;

        [SetUp]
        public void SetUp()
        {
            packageFolder = FileSystem.Combine("../../../TestPackage1").ToFullPath();

            var fileSystem = new FileSystem();
            var manifest = new PackageManifest(){
                Assemblies = "TestPackage1",
                Name = "pak1"
            };

            fileSystem.PersistToFile(manifest, packageFolder, PackageManifest.FILE);

            reader = new PackageManifestReader("../../".ToFullPath(), fileSystem);
        }

        [TearDown]
        public void TearDown()
        {
            new FileSystem().DeleteFile("../../".ToFullPath(), PackageIncludeManifest.FILE);
        }

        [Test]
        public void load_a_package_info_from_a_manifest_file_when_given_the_folder()
        {
            // the reader is rooted at the folder location of the main app
            var package = reader.LoadFromFolder("../../../TestPackage1".ToFullPath());

            package.Assemblies.Single().GetName().Name.ShouldEqual("TestPackage1");
            package.FilesFolder.ShouldEqual(packageFolder);
        }

        [Test]
        public void load_all_packages_by_reading_the_include_folder()
        {
            var includes = new PackageIncludeManifest();
            includes.Include("../TestPackage1");

            new FileSystem().PersistToFile(includes, "../../".ToFullPath(), PackageIncludeManifest.FILE);

            var package = reader.ReadAll().Single();

            package.Assemblies.Single().GetName().Name.ShouldEqual("TestPackage1");
            package.FilesFolder.ToLower().ShouldEqual(packageFolder.ToLower());
        }
    }
}