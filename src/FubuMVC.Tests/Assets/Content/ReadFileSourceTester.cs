using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Content
{
    [TestFixture]
    public class ReadFileSourceTester
    {
        [Test]
        public void files_are_just_the_one_file()
        {
            var file = new AssetFile("something.js");
            var source = new ReadFileSource(file);

            source.Files.Single().ShouldBeTheSameAs(file);
        }

        [Test]
        public void read_content()
        {
            var context = MockRepository.GenerateMock<ITransformContext>();
            var file = new AssetFile("something.js"){
                FullPath = "some/path"
            };

            var theContents = "some contents";
            context.Stub(x => x.ReadContentsFrom(file.FullPath)).Return(theContents);

            var source = new ReadFileSource(file);

            source.GetContent(context).ShouldEqual(theContents);
        }
    }


}