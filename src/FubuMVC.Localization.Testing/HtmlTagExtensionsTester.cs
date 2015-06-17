using FubuLocalization;
using FubuMVC.Core.UI;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Localization.Testing
{
    [TestFixture]
    public class HtmlTagExtensionsTester
    {
        [Test]
        public void set_localized_tag_text_by_string_token()
        {
            var token = StringToken.FromKeyString("KEY", "the text of this string token");
            new HtmlTag("a").Text(token)
                .Text().ShouldEqual(token.ToString());
        }

        [Test]
        public void set_localized_attr_value_by_string_token()
        {
            var token = StringToken.FromKeyString("KEY", "the text of this string token");

            new HtmlTag("span").Attr("title", token)
                .Attr("title").ShouldEqual(token.ToString());
        }

        [Test]
        public void set_the_text_by_localized_StringToken()
        {
            var token = StringToken.FromKeyString("KEY", "the localized string");
            new HtmlTag("div").TextIfEmpty(token)
                .Text().ShouldEqual(token.ToString());
        }
    }
}