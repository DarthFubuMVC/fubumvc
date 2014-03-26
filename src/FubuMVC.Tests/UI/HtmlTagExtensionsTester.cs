using System;
using System.Collections.Generic;
using FubuMVC.Core.UI;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class HtmlTagExtensionsTester
    {

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

        [Test]
        public void mustache_attr()
        {
            var tag = new HtmlTag("a");
            tag.MustacheAttr("href", "url");

            tag.ToString().ShouldEqual("<a href=\"{{url}}\"></a>");
        }

        [Test]
        public void mustache_value()
        {
            var tag = new TextboxTag();
            tag.MustacheValue("prop");

            tag.ToString().ShouldEqual("<input type=\"text\" value=\"{{prop}}\" />");
        }

        [Test]
        public void mustache_text()
        {
            var tag = new HtmlTag("span");
            tag.MustacheText("prop");

            tag.ToString().ShouldEqual("<span>{{prop}}</span>");
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

    }
}