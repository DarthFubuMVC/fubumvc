using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Content
{
    [TestFixture]
    public class FileReadTester
    {
        [Test]
        public void files_are_just_the_one_file()
        {
            var file = new AssetFile("something.js");
            var source = new FileRead(file);

            source.Files.Single().ShouldBeTheSameAs(file);
        }

        [Test]
        public void to_string_because_automated_tests_depend_on_it()
        {
            var file = new AssetFile("something.js");
            var read = new FileRead(file);
            read.ToString().ShouldEqual("FileRead:something.js");
        }

        [Test]
        public void no_inner_content_sources()
        {
            var file = new AssetFile("something.js");
            var source = new FileRead(file);

            source.InnerSources.Any().ShouldBeFalse();
        }

        [Test]
        public void read_content()
        {
            var context = MockRepository.GenerateMock<IContentPipeline>();
            var file = new AssetFile("something.js"){
                FullPath = "some/path"
            };

            var theContents = "some contents";
            context.Stub(x => x.ReadContentsFrom(file.FullPath)).Return(theContents);

            var source = new FileRead(file);

            source.GetContent(context).ShouldEqual(theContents);
        }
    }


}