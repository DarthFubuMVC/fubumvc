using System.Linq;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.TestSupport;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Resources.Conneg
{
    [TestFixture]
    public class HtmlStringWriterTester : InteractionContext<HtmlStringWriter<HtmlTag>>
    {
        private HtmlTag theTag;

        protected override void beforeEach()
        {
            theTag = new HtmlTag("div");
        }

        [Test]
        public void the_only_mime_type_is_html()
        {
            ClassUnderTest.Mimetypes.Single()
                .ShouldBe(MimeType.Html.Value);
        }

        [Test]
        public void writing_should_write_the_to_string_of_the_target()
        {
            var context = new MockedFubuRequestContext(Services.Container);

            ClassUnderTest.Write(MimeType.Html.Value, context, theTag);

            context.Writer.AssertWasCalled(x => x.Write(MimeType.Html.Value, theTag.ToString()));
        }
    }
}