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
        protected override void beforeEach()
        {
            ClassUnderTest.RegisterDirectory("a");
            ClassUnderTest.RegisterDirectory("b");
            ClassUnderTest.RegisterDirectory("c\\d");


        }

        [Test]
        public void return_null_for_any_image_if_there_is_no_registerd_package_content_folders()
        {
            ClassUnderTest.FileNameFor("something.png").ShouldBeNull();
        }

        [Test]
        public void return_the_image_file_found_at_the_first_package()
        {
            hasFile("a", "something.png");
            ClassUnderTest.FileNameFor("something.png").ShouldEqual("a\\something.png");
        }

        [Test]
        public void return_the_image_file_found_at_the_first_package_2()
        {
            hasFile("b", "something.png");
            ClassUnderTest.FileNameFor("something.png").ShouldEqual("b\\something.png");
        }

        [Test]
        public void return_the_image_file_found_at_the_first_package_3()
        {
            hasFile("c\\d", "something.png");
            ClassUnderTest.FileNameFor("something.png").ShouldEqual("c\\d\\something.png");
        }

        [Test]
        public void return_null_for_the_url_for_any_image_if_there_is_no_registerd_package_content_folders()
        {
            ClassUnderTest.UrlFor("something.png").ShouldBeNull();
        }

        [Test]
        public void return_url_for_the_image_file_found_at_the_first_package()
        {
            hasFile("a", "something.png");
            ClassUnderTest.UrlFor("something.png").ShouldEqual("~/_images/something.png");
        }

        [Test]
        public void return_url_for_the_image_file_found_at_the_first_package_2()
        {
            hasFile("b", "something.png");
            ClassUnderTest.UrlFor("something.png").ShouldEqual("~/_images/something.png");
        }

        [Test]
        public void return_url_for_the_image_file_found_at_the_first_package_3()
        {
            hasFile("c\\d", "something.png");
            ClassUnderTest.UrlFor("something.png").ShouldEqual("~/_images/something.png");
        }

        [Test]
        public void return_url_for_the_image_file_found_at_the_first_package_4()
        {
            hasFile("c\\d", "e/j/something.png");
            ClassUnderTest.UrlFor("e/j/something.png").ShouldEqual("~/_images/e/j/something.png");
        }    


        private void hasFile(string directory, string fileName)
        {
            MockFor<IFileSystem>().Stub(x => x.FileExists(directory, fileName)).Return(true);
            MockFor<IFileSystem>().Stub(x => x.Combine(directory, fileName)).Return(Path.Combine(directory, fileName));
        }
    }
}