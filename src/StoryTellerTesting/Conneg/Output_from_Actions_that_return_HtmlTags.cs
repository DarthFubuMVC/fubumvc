using System;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using HtmlTags;
using NUnit.Framework;

namespace IntegrationTesting.Conneg
{
    [TestFixture]
    public class Output_from_Actions_that_return_HtmlTags : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<HtmlTagController>();
        }

        [Test]
        public void getting_text_from_an_HtmlTag_action()
        {
            endpoints.Get<HtmlTagController>(x => x.get_tag())
                .ContentShouldBe(MimeType.Html, new HtmlTagController().get_tag().ToString());
        }

        [Test]
        public void getting_text_from_an_HtmlDocument_action()
        {
            endpoints.Get<HtmlTagController>(x => x.get_document())
                .ContentShouldBe(MimeType.Html, new HtmlTagController().get_document().ToString());
        }
    }

    public class HtmlTagController
    {
        public HtmlTag get_tag()
        {
            return new HtmlTag("div").Text("hello.");
        }

        public HtmlDocument get_document()
        {
            return new HtmlDocument(){
                Title = "This is a document"
            };
        }
    }
}