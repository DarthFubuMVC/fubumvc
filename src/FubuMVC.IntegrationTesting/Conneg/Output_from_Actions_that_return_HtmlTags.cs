using FubuMVC.Core.Runtime;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Conneg
{
    [TestFixture]
    public class Output_from_Actions_that_return_HtmlTags
    {
        [Test]
        public void getting_text_from_an_HtmlTag_action()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<HtmlTagEndpoints>(x => x.get_tag());
                _.ContentTypeShouldBe(MimeType.Html);
                _.ContentShouldBe(new HtmlTagEndpoints().get_tag().ToString());
            });
        }

        [Test]
        public void getting_text_from_an_HtmlDocument_action()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<HtmlTagEndpoints>(x => x.get_document());
                _.ContentTypeShouldBe(MimeType.Html);
                _.ContentShouldBe(new HtmlTagEndpoints().get_document().ToString());
            });
        }
    }

    public class HtmlTagEndpoints
    {
        public HtmlTag get_tag()
        {
            return new HtmlTag("div").Text("hello.");
        }

        public HtmlDocument get_document()
        {
            return new HtmlDocument
            {
                Title = "This is a document"
            };
        }
    }
}