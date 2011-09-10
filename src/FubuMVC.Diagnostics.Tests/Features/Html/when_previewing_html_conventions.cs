using FubuMVC.Diagnostics.Features.Html;
using FubuMVC.Diagnostics.Features.Html.Preview;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Features.Html
{
    [TestFixture]
    public class when_previewing_html_conventions : InteractionContext<get_OutputModel_handler>
    {
        private HtmlConventionsPreviewRequestModel _request;

        protected override void beforeEach()
        {
            _request = new HtmlConventionsPreviewRequestModel
                           {
                               OutputModel = "test"
                           };
        }

        [Test]
        public void should_establish_preview_context()
        {
            MockFor<IHtmlConventionsPreviewContextFactory>()
                .Expect(factory => factory.BuildFromPath(_request.OutputModel))
                .Return(ObjectMother.BasicPreviewContext());

            ClassUnderTest
                .Execute(_request);

            VerifyCallsFor<IHtmlConventionsPreviewContextFactory>();
        }

        [Test]
        public void should_enrich_preview_model()
        {
            var context = ObjectMother.BasicPreviewContext();
            MockFor<IHtmlConventionsPreviewContextFactory>()
                .Expect(factory => factory.BuildFromPath(_request.OutputModel))
                .Return(context);

            MockFor<IPreviewModelDecorator>()
                .Expect(d => d.Enrich(Arg<HtmlConventionsPreviewContext>.Is.Same(context), 
                    Arg<HtmlConventionsPreviewViewModel>.Is.NotNull));

            ClassUnderTest.Execute(_request);

            VerifyCallsFor<IPreviewModelDecorator>();
        }
    }
}