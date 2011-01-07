using System.IO;
using FubuCore;
using FubuMVC.Core.Content;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Content
{
    [TestFixture]
    public class PackagedImageUrlResolverTester : InteractionContext<PackagedImageUrlResolver>
    {


        [Test]
        public void return_null_for_the_url_for_any_image_if_there_is_no_registerd_package_content_folders()
        {
            ClassUnderTest.UrlFor("something.png").ShouldBeNull();
        }

        [Test]
        public void return_url_for_the_image_file_found_at_the_first_package()
        {
            hasFile("something.png");
            ClassUnderTest.UrlFor("something.png").ShouldEqual("~/_images/something.png");
        }

        [Test]
        public void return_url_for_the_image_file_found_at_the_first_package_2()
        {
            hasFile("something.png");
            ClassUnderTest.UrlFor("something.png").ShouldEqual("~/_images/something.png");
        }

        [Test]
        public void return_url_for_the_image_file_found_at_the_first_package_3()
        {
            hasFile("something.png");
            ClassUnderTest.UrlFor("something.png").ShouldEqual("~/_images/something.png");
        }

        [Test]
        public void return_url_for_the_image_file_found_at_the_first_package_4()
        {
            hasFile("e/j/something.png");
            ClassUnderTest.UrlFor("e/j/something.png").ShouldEqual("~/_images/e/j/something.png");
        }


        private void hasFile(string fileName)
        {
            MockFor<IContentFolderService>().Stub(x => x.FileExists(ContentType.images, fileName)).Return(true);
        }
    }
}