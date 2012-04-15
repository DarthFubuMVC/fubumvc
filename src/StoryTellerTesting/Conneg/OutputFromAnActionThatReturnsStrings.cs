using System;
using System.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Serenity.Endpoints;

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
            endpoints.Get<StringController>(x => x.SayHello())
                .ContentShouldBe(MimeType.Text, "Hello.");
        }

        [Test]
        public void action_that_returns_a_string_from_a_method_that_should_be_html()
        {
            endpoints.Get<StringController>(x => x.SayHelloWithHtml())
                .ContentShouldBe(MimeType.Html, "<h1>Hello</h1>");
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