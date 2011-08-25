using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace FubuMVC.Tests.Assets.Content
{
    [TestFixture]
    public class TransformTester : InteractionContext<Transform<StubTransformer>>
    {
        [Test]
        public void files_delegates_to_the_inner()
        {
            var files = new AssetFile[0];

            MockFor<IContentSource>().Stub(x => x.Files).Return(files);

            ClassUnderTest.Files.ShouldBeTheSameAs(files);
        }

        [Test]
        public void to_string_because_it_matters_for_testing()
        {
            ClassUnderTest.ToString().ShouldEqual("Transform:" + typeof (StubTransformer).Name);
        }

        [Test]
        public void inner_sources_includes_only_the_one_inner_source()
        {
            ClassUnderTest.InnerSources.Single().ShouldBeTheSameAs(MockFor<IContentSource>());
        }

        [Test]
        public void get_content_invokes_the_corrent_asset_transformer_against_the_content_of_the_inner_source()
        {
            var theTransformerFoundFromThePipeline = MockFor<ITransformer>();
            var thePipeline = MockFor<IContentPipeline>();

            var files = new AssetFile[0];

            var theInnerContents = "inner contents";
            var theInnerContentSource = MockFor<IContentSource>();
            theInnerContentSource.Stub(x => x.Files).Return(files);
            theInnerContentSource.Stub(x => x.GetContent(thePipeline)).Return(theInnerContents);


            thePipeline.Stub(x => x.GetTransformer<StubTransformer>()).Return(theTransformerFoundFromThePipeline);

            
            var theTransformedContents = "transformed contents";
            theTransformerFoundFromThePipeline.Stub(x => x.Transform(theInnerContents, files)).Return(theTransformedContents);


            ClassUnderTest.GetContent(thePipeline).ShouldEqual(theTransformedContents);
        }
    }

    
}