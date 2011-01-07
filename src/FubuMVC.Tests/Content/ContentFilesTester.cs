using System.IO;
using FubuCore;
using FubuMVC.Core.Content;
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
            theContentFiles["something.png"].ShouldEqual("~/content/images/something.png");
        }

        [Test]
        public void return_url_for_the_image_file_found_at_the_first_package()
        {
            hasFile("something.png");
            theContentFiles["something.png"].ShouldEqual("~/_images/something.png");
        }

        [Test]
        public void return_url_for_the_image_file_found_at_the_first_package_2()
        {
            hasFile("something.png");
            theContentFiles["something.png"].ShouldEqual("~/_images/something.png");
        }

        [Test]
        public void return_url_for_the_image_file_found_at_the_first_package_3()
        {
            hasFile("something.png");
            theContentFiles["something.png"].ShouldEqual("~/_images/something.png");
        }

        [Test]
        public void return_url_for_the_image_file_found_at_the_first_package_4()
        {
            hasFile("e/j/something.png");
            theContentFiles["e/j/something.png"].ShouldEqual("~/_images/e/j/something.png");
        }


        private void hasFile(string fileName)
        {
            folders.Stub(x => x.FileExists(ContentType.images, fileName)).Return(true);
        }
    }
}