using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.TestingHarness;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Conneg
{
    [TestFixture]
    public class OutputFromAnActionThatReturnsStrings : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<StringController>();
        }

        [Test]
        public void action_that_returns_a_string_should_be_formatted_as_plain_text()
        {
            endpoints.Get<StringController>(x => x.SayHello())
                .ContentShouldBe(MimeType.Text, "Hello.");
        }

        [Test]
        public void action_that_returns_a_string_from_a_method_that_should_be_html()
        {
            endpoints.Get<StringController>(x => x.SayHelloWithHtml(), acceptType:"text/html")
                .ContentShouldBe(MimeType.Html, "<h1>Hello</h1>");
        }

        [Test]
        public void action_marked_with_html_endpoint_should_be_written_as_html()
        {
            endpoints.Get<StringController>(x => x.DifferentKindOfName(), acceptType:"text/html")
                .ContentShouldBe(MimeType.Html, "different");
        }

        [Test]
        public void action_marked_with_html_endpoint_that_returns_string_still_returns_text_when_text_is_requested()
        {
            endpoints.Get<StringController>(x => x.DifferentKindOfName(), acceptType:"text/plain")
                .ContentShouldBe(MimeType.Text, "different");
        }

    }


    public class StringController
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