using FubuCore;
using FubuMVC.Core.Content;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Content
{
    [TestFixture]
    public class ContentFolderServiceTester : InteractionContext<ContentFolderService>
    {
        protected override void beforeEach()
        {
            ClassUnderTest.RegisterDirectory("a");
            ClassUnderTest.RegisterDirectory("b");
            ClassUnderTest.RegisterDirectory("c\\d");


        }

        private void hasFile(string directory, string fileName, ContentType contentType)
        {
            MockFor<IFileSystem>().Stub(x => FileSystemExtensions.FileExists(x, directory, contentType.ToString(), fileName)).Return(true);
        }

        [Test]
        public void return_null_for_any_image_if_there_is_no_registerd_package_content_folders()
        {
            ClassUnderTest.FileNameFor(ContentType.images, "something.png").ShouldBeNull();
        }

        [Test]
        public void return_the_image_file_found_at_the_first_package()
        {
            hasFile("a", "something.png", ContentType.images);
            ClassUnderTest.FileNameFor(ContentType.images, "something.png").ShouldEqual("a\\images\\something.png");
        }

        [Test]
        public void return_the_image_file_found_at_the_first_package_2()
        {
            hasFile("b", "something.png", ContentType.images);
            ClassUnderTest.FileNameFor(ContentType.images, "something.png").ShouldEqual("b\\images\\something.png");
        }

        [Test]
        public void return_the_image_file_found_at_the_first_package_3()
        {
            hasFile("c\\d", "something.png", ContentType.images);
            ClassUnderTest.FileNameFor(ContentType.images, "something.png").ShouldEqual("c\\d\\images\\something.png");
        }

        [Test]
        public void has_file_positive_case()
        {
            hasFile("b", "something.png", ContentType.images);
            ClassUnderTest.FileExists(ContentType.images, "something.png").ShouldBeTrue();
        }



        [Test]
        public void has_file_negative_case()
        {
            ClassUnderTest.FileExists(ContentType.images, "something.png").ShouldBeFalse();
        }


        [Test]
        public void has_file_negative_case_2()
        {
            hasFile("a", "something.png", ContentType.scripts);
            ClassUnderTest.FileExists(ContentType.images, "something.png").ShouldBeFalse();
        }
    }
}