using Bottles;
using Fubu;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using FileSystemExtensions = FubuCore.FileSystemExtensions;

namespace FubuMVC.Tests.Commands
{
    [TestFixture]
    public class ManifestCommandTester : InteractionContext<ManifestCommand>
    {
        private ManifestInput theInput;
        private PackageManifest theManifest;

        protected override void beforeEach()
        {
            theInput = new ManifestInput(){
                AppFolder = "some folder"
            };

            theManifest = new PackageManifest();

        }

        private void theManifestFileExists()
        {
            MockFor<IFileSystem>().Stub(x => FileSystemExtensions.FileExists(x, theInput.AppFolder, PackageManifest.FILE)).Return(true);
            MockFor<IFileSystem>().Stub(x => FileSystemExtensions.LoadFromFile<PackageManifest>(x, theInput.AppFolder, PackageManifest.FILE)).Return(theManifest);
        }


        [Test]
        public void the_file_exists_and_the_open_flag_is_true()
        {
            theManifestFileExists();
            theInput.OpenFlag = true;

            ClassUnderTest.Execute(theInput, MockFor<IFileSystem>());

            MockFor<IFileSystem>().AssertWasCalled(x => FileSystemExtensions.LaunchEditor(x, theInput.AppFolder, PackageManifest.FILE));
        }
    }
}