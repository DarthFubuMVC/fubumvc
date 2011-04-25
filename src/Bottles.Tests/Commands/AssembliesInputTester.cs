using Bottles.Commands;
using Bottles.Tests.Parsing;
using FubuCore;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace Bottles.Tests.Commands
{
    [TestFixture]
    public class AssembliesInputTester
    {
        private IFileSystem theFileSystem;
        private AssembliesInput theInput;
        private PackageManifest thePackageManifest;
        private PackageManifest theApplicationManifest;

        [SetUp]
        public void SetUp()
        {
            theFileSystem = MockRepository.GenerateMock<IFileSystem>();
            theInput = new AssembliesInput{
                Directory = "directory1",
                File = null
            };

            thePackageManifest = new PackageManifest(){
                Name = "the package"
            };
            theApplicationManifest = new PackageManifest(){
                Name = "the application"
            };
        }

        private void thePackageManifestFileExists()
        {
            theFileSystem.Stub(
                x => x.FileExists(theInput.Directory, PackageManifest.FILE))
                .Return(true);

            theFileSystem.Stub(
                x =>
                x.LoadFromFile<PackageManifest>(theInput.Directory, PackageManifest.FILE))
                .Return(thePackageManifest);
        }

        private void theApplicationManifestFileExists()
        {
            theFileSystem.Stub(
                x => x.FileExists(theInput.Directory, PackageManifest.APPLICATION_MANIFEST_FILE))
                .Return(true);

            theFileSystem.Stub(
                x =>
                x.LoadFromFile<PackageManifest>(theInput.Directory, PackageManifest.APPLICATION_MANIFEST_FILE))
                .Return(theApplicationManifest);
        }

        [Test]
        public void find_the_package_manifest_file()
        {
            thePackageManifestFileExists();
            theInput.FindManifestAndBinaryFolders(theFileSystem);

            theInput.Manifest.ShouldBeTheSameAs(thePackageManifest);

            theInput.File.ShouldEqual(FileSystem.Combine(theInput.Directory, PackageManifest.FILE));
        }


        [Test]
        public void find_the_application_manifest_file()
        {
            theApplicationManifestFileExists();
            theInput.FindManifestAndBinaryFolders(theFileSystem);

            theInput.Manifest.ShouldBeTheSameAs(theApplicationManifest);

            theInput.File.ShouldEqual(FileSystem.Combine(theInput.Directory, PackageManifest.APPLICATION_MANIFEST_FILE));
        }

        [Test]
        public void use_the_file_name_if_it_is_given_and_the_manifest_exists_at_that_file()
        {
            var manifest = new PackageManifest(){
                Name = "special"
            };

            theInput.File = "special.xml";

            theFileSystem.Stub(x => x.FileExists(theInput.Directory, theInput.File))
                .Return(true);

            theFileSystem.Stub(x => x.LoadFromFile<PackageManifest>(theInput.Directory, theInput.File))
                .Return(manifest);

            theInput.FindManifestAndBinaryFolders(theFileSystem);

            theInput.File.ShouldEqual(FileSystem.Combine(theInput.Directory, "special.xml"));
            theInput.Manifest.ShouldBeTheSameAs(manifest);
        }
    }
}