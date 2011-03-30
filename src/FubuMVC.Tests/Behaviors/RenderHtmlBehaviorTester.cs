using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Behaviors
{
    [TestFixture]
    public class when_executing_the_render_html_behavior : InteractionContext<RenderHtmlBehavior<HtmlDocument>>
    {
        private HtmlDocument document;

        protected override void beforeEach()
        {
            document = new HtmlDocument();
            var request = new InMemoryFubuRequest();
            Services.Inject<IFubuRequest>(request);

            request.Set(document);

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_write_the_string_contents_of_the_HtmlDocument_in_the_FubuRequest()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.Write(MimeType.Html.ToString(), document.ToString()));
        }
    }
}