using System;
using System.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace IntegrationTesting.Conneg
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
            var response = endpoints.Get<StringController>(x => x.SayHello());

            response.ContentType.ShouldEqual(MimeType.Text.Value);
            var text = response.ReadAsText();

            text.ShouldEqual("Hello.");
        }

        [Test]
        public void action_that_returns_a_string_from_a_method_that_should_be_html()
        {
            var response = endpoints.Get<StringController>(x => x.SayHelloWithHtml());

            response.ContentType.ShouldEqual(MimeType.Html.Value);
            var text = response.ReadAsText();

            text.ShouldEqual("<h1>Hello</h1>");
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
    }
}