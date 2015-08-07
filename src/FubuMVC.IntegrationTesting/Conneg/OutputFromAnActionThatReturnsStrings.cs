using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Conneg
{
    [TestFixture]
    public class OutputFromAnActionThatReturnsStrings 
    {

        [Test]
        public void action_that_returns_a_string_should_be_formatted_as_plain_text()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<StringWritingEndpoints>(x => x.SayHello());
                _.ContentTypeShouldBe(MimeType.Text);
                _.ContentShouldBe("Hello.");
            });
        }

        [Test]
        public void action_that_returns_a_string_from_a_method_that_should_be_html()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<StringWritingEndpoints>(x => x.SayHelloWithHtml()).Accepts("text/html");
                _.ContentTypeShouldBe(MimeType.Html);
                _.ContentShouldBe("<h1>Hello</h1>");
            });
        }

        [Test]
        public void action_marked_with_html_endpoint_should_be_written_as_html()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<StringWritingEndpoints>(x => x.DifferentKindOfName()).Accepts("text/html");
                _.ContentTypeShouldBe(MimeType.Html);
                _.ContentShouldBe("different");
            });
        }

        [Test]
        public void action_marked_with_html_endpoint_that_returns_string_still_returns_text_when_text_is_requested()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<StringWritingEndpoints>(x => x.DifferentKindOfName()).Accepts("text/plain");
                _.ContentTypeShouldBe(MimeType.Text);
                _.ContentShouldBe("different");
            });
        }
    }


    public class StringWritingEndpoints
    {
        public string SayHello()
        {
            return "Hello.";
        }

        public string SayHelloWithHtml()
        {
            return "<h1>Hello</h1>";
        }

        public string DifferentKindOfName()
        {
            return "different";
        }

        public Something AsHtml()
        {
            return new Something();
        }
    }

    public class Something
    {
    }
}