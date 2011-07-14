using System.IO;
using FubuCore;
using FubuMVC.Core.Content;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Content
{
    [TestFixture]
    public class ContentFilesTester
    {
        private IContentFolderService folders;
        private ContentFiles theContentFiles;

        [SetUp]
        public void SetUp()
        {
            folders = MockRepository.GenerateMock<IContentFolderService>();
            theContentFiles = new ContentFiles(folders, ContentType.images);
        }

        [Test]
        public void return_straight_path_for_the_url_for_any_image_if_exists_in_main_application()
        {
            hasFile("something.png");
            folders.Stub(x => x.ExistsInApplicationDirectory(ContentType.images, "something.png")).Return(true);
            theContentFiles.File("something.png").ShouldEqual("~/content/images/something.png");
        }

        [Test]
        public void return_url_for_the_image_file_found_at_the_first_package()
        {
            hasFile("something.png");
            theContentFiles.File("something.png").ShouldEqual("~/_content/images/something.png");
        }

        [Test]
        public void return_url_for_the_image_file_found_at_the_first_package_in_a_deeper_path()
        {
            hasFile("e/j/something.png");
            theContentFiles.File("e/j/something.png").ShouldEqual("~/_content/images/e/j/something.png");
        }

        [Test]
        public void return_null_if_optional_file_does_not_exist()
        {
            theContentFiles.File("something.png", true).ShouldBeNull();
        }

        [Test]
        public void return_path_from_main_application_if_file_does_not_exist_and_not_optional()
        {
            theContentFiles.File("something.png", true).ShouldBeNull();
        }


        private void hasFile(string fileName)
        {
            folders.Stub(x => x.FileExists(ContentType.images, fileName)).Return(true);
        }
    }
}