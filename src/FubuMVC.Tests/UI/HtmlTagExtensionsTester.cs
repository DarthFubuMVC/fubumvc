using System;
using System.Diagnostics;
using FubuLocalization;
using HtmlTags;
using NUnit.Framework;
using FubuMVC.Core.UI;
using System.Linq;
using System.Collections.Generic;

namespace FubuMVC.Tests.UI
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
        public void find_first_child()
        {
            var tag = new HtmlTag("a");
            tag.Add("span");
            tag.Add("span");
            var div = tag.Add("div");
            tag.Add("input");
            tag.Add("button");

            

            new TagHolder(tag).ForChild("div").ShouldBeTheSameAs(div);
        }

        public class TagHolder : ITagSource
        {
            private readonly HtmlTag _inner;

            public TagHolder(HtmlTag inner)
            {
                _inner = inner;
            }

            public IEnumerable<HtmlTag> AllTags()
            {
                return _inner.Children;
            }
        }
    }



    [TestFixture]
    public class TextIfEmptyTester
    {
        [Test]
        public void trying_to_set_the_default_text_on_an_input_element_should_throw_an_exception()
        {
            Exception<InvalidOperationException>.ShouldBeThrownBy(() =>
            {
                new HtmlTag("input").TextIfEmpty("something");
            });
        }

        [Test]
        public void do_nothing_if_the_tag_already_has_text()
        {
            new HtmlTag("div").Text("original").TextIfEmpty("different")
                .Text().ShouldEqual("original");
        }

        [Test]
        public void set_the_text_if_the_tag_text_is_initially_null()
        {
            new HtmlTag("div").TextIfEmpty("defaulted")
                .Text().ShouldEqual("defaulted");
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