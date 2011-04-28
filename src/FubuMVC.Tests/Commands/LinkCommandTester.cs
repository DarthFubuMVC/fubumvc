using System.IO;
using Bottles;
using Fubu;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Commands
{
    [TestFixture]
    public class LinkCommandTester : InteractionContext<LinkCommand>
    {
        private LinkInput theInput;
        private PackageManifest appManifest;
        private PackageManifest pakManifest;

        protected override void beforeEach()
        {
            theInput = new LinkInput
            {
                PackageFolder = "package",
                AppFolder = "app",
            };

            appManifest = new PackageManifest();
            pakManifest = new PackageManifest();
            Services.PartialMockTheClassUnderTest();
        }
        
        private void theManifestFileExists()
        {
            var packageManifestFileName = FileSystem.Combine(theInput.PackageFolder, PackageManifest.FILE);
            MockFor<IFileSystem>().Stub(x => x.FileExists(packageManifestFileName)).Return(true);

            MockFor<IFileSystem>().Stub(x => x.PackageManifestExists(theInput.PackageFolder)).Return(true);
            MockFor<IFileSystem>().Stub(x => x.LoadFromFile<PackageManifest>(packageManifestFileName)).Return(pakManifest);
            MockFor<IFileSystem>().Stub(x => x.FileExists(theInput.AppFolder, PackageManifest.FILE)).Return(true);
            MockFor<IFileSystem>().Stub(x => x.LoadFromFile<PackageManifest>(theInput.AppFolder, PackageManifest.FILE)).Return(appManifest);
        }

        private string oneFolderUp(string path)
        {
            return "..{0}{1}".ToFormat(Path.DirectorySeparatorChar, path);
        }

        private void execute()
        {
            ClassUnderTest.Execute(theInput, MockFor<IFileSystem>());
        }

        [Test]
        public void should_link_app_to_package()
        {
            var expectedFolder = oneFolderUp(theInput.PackageFolder);
            theManifestFileExists();

            execute();

            appManifest.LinkedFolders.ShouldContain(expectedFolder);
        }

        [Test]
        public void should_link_app_to_package_with_trailing_slash_for_app()
        {
            var expectedFolder = oneFolderUp(theInput.PackageFolder);
            theInput.AppFolder += Path.DirectorySeparatorChar;
            theManifestFileExists();

            execute();

            appManifest.LinkedFolders.ShouldContain(expectedFolder);
        }
    }
}