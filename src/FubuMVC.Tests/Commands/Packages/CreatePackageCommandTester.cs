using Fubu.Packages;
using FubuCore;
using FubuMVC.Core.Packaging;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Commands.Packages
{
    [TestFixture]
    public class CreatePackageCommandTester : InteractionContext<CreatePackageCommand>
    {
        private CreatePackageInput theInput;
        private PackageManifest theManifest;

        protected override void beforeEach()
        {
            theInput = new CreatePackageInput()
            {
                PackageFolder = "some folder",
                ZipFile = "c:\\package1.zip"
            };

            theManifest = new PackageManifest();
            Services.PartialMockTheClassUnderTest();
        }


        private void theManifestFileDoesNotExist()
        {
            MockFor<IFileSystem>().Stub(x => x.PackageManifestExists(theInput.PackageFolder)).Return(false);
        }

        private void theManifestFileExists()
        {
            MockFor<IFileSystem>().Stub(x => x.PackageManifestExists(theInput.PackageFolder)).Return(true);
            MockFor<IFileSystem>().Stub(x => x.LoadPackageManifestFrom(theInput.PackageFolder)).Return(theManifest);
        }

        private void theZipFileAlreadyExists()
        {
            MockFor<IFileSystem>().Stub(x => x.FileExists(theInput.ZipFile)).Return(true);
        }

        private void theZipFileDoesNotExist()
        {
            MockFor<IFileSystem>().Stub(x => x.FileExists(theInput.ZipFile)).Return(false);
        }

        private void execute()
        {
            ClassUnderTest.Execute(theInput, MockFor<IFileSystem>());
        }

        [Test]
        public void delete_the_existing_package_zip_file_if_it_exists_and_force_is_true()
        {
            theManifestFileExists();
            theZipFileAlreadyExists();
            theInput.ForceFlag = true;

            // Just forcing this method to be self-mocked
            ClassUnderTest.Expect(x => x.CreatePackage(theInput, MockFor<IFileSystem>()));
            ClassUnderTest.Expect(x => x.WriteZipFileAlreadyExists(theInput.ZipFile));

            execute();


            ClassUnderTest.AssertWasNotCalled(x => x.WriteZipFileAlreadyExists(theInput.ZipFile));
            MockFor<IFileSystem>().AssertWasCalled(x => x.DeleteFile(theInput.ZipFile));

        }

        [Test]
        public void do_not_delete_the_existing_zip_file_if_it_exists_and_force_flag_is_false()
        {
            theManifestFileExists();
            theZipFileAlreadyExists();
            theInput.ForceFlag = false;

            // Just forcing this method to be self-mocked
            ClassUnderTest.Expect(x => x.CreatePackage(theInput, MockFor<IFileSystem>()));
            ClassUnderTest.Expect(x => x.WriteZipFileAlreadyExists(theInput.ZipFile));

            execute();

            MockFor<IFileSystem>().AssertWasNotCalled(x => x.DeleteFile(theInput.ZipFile));
        
            ClassUnderTest.AssertWasNotCalled(x => x.CreatePackage(theInput, MockFor<IFileSystem>()));
            ClassUnderTest.AssertWasCalled(x => x.WriteZipFileAlreadyExists(theInput.ZipFile));
        }

        [Test]
        public void create_the_package_if_the_package_manifest_file_exists()
        {
            theManifestFileExists();
            theZipFileDoesNotExist();

            // Just forcing this method to be self-mocked
            ClassUnderTest.Expect(x => x.CreatePackage(theInput, MockFor<IFileSystem>()));
            ClassUnderTest.Expect(x => x.WritePackageManifestDoesNotExist(theInput.PackageFolder)).Repeat.Never();

            execute();

            ClassUnderTest.VerifyAllExpectations();
        }

        [Test]
        public void do_not_create_the_package_if_the_package_manifest_file_does_not_exist()
        {
            theManifestFileDoesNotExist();
            theZipFileDoesNotExist();

            // Just forcing this method to be self-mocked
            ClassUnderTest.Expect(x => x.CreatePackage(theInput, MockFor<IFileSystem>())).Repeat.Never();
            ClassUnderTest.Expect(x => x.WritePackageManifestDoesNotExist(theInput.PackageFolder));

            execute();

            ClassUnderTest.VerifyAllExpectations();
        }
    }
}