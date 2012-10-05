using FubuCore;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Tests.Assets.Content;
using FubuTestingSupport;

using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Http
{
    [TestFixture]
    public class ContentPipelineTester : InteractionContext<ContentPipeline>
    {
        [Test]
        public void get_transformer_delegates_to_the_service_locator()
        {
            var transformer = new StubTransformer();
            MockFor<IServiceLocator>().Stub(x => x.GetInstance<StubTransformer>())
                .Return(transformer);

            ClassUnderTest.GetTransformer<StubTransformer>().ShouldBeTheSameAs(transformer);
        }

        [Test]
        public void read_contents_from_file()
        {
            var file = "some file";
            var theContents = "some contents";

            MockFor<IFileSystem>().Stub(x => x.ReadStringFromFile(file))
                .Return(theContents);

            ClassUnderTest.ReadContentsFrom(file).ShouldEqual(theContents);
        }
    }
}