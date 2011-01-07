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

        private void hasFile(string directory, string fileName)
        {
            MockFor<IFileSystem>().Stub(x => FileSystemExtensions.FileExists(x, directory, fileName)).Return(true);
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
        public void has_file_positive_case()
        {
            hasFile("b", "something.png");
            ClassUnderTest.FileExists("something.png").ShouldBeTrue();
        }

        [Test]
        public void has_file_negative_case()
        {
            ClassUnderTest.FileExists("something.png").ShouldBeFalse();
        }
    }
}