using System;
using System.Collections.Generic;
using FubuMVC.Core.View;
using Shouldly;
using HtmlTags;
using Xunit;

namespace FubuMVC.Tests.UI
{
    
    public class HtmlTagExtensionsTester
    {

        [Fact]
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

        [Fact]
        public void mustache_attr()
        {
            var tag = new HtmlTag("a");
            tag.MustacheAttr("href", "url");

            tag.ToString().ShouldBe("<a href=\"{{url}}\"></a>");
        }

        [Fact]
        public void mustache_value()
        {
            var tag = new TextboxTag();
            tag.MustacheValue("prop");

            tag.ToString().ShouldBe("<input type=\"text\" value=\"{{prop}}\" />");
        }

        [Fact]
        public void mustache_text()
        {
            var tag = new HtmlTag("span");
            tag.MustacheText("prop");

            tag.ToString().ShouldBe("<span>{{prop}}</span>");
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



    
    public class TextIfEmptyTester
    {
        [Fact]
        public void trying_to_set_the_default_text_on_an_input_element_should_throw_an_exception()
        {
            Exception<InvalidOperationException>.ShouldBeThrownBy(() =>
            {
                new HtmlTag("input").TextIfEmpty("something");
            });
        }

        [Fact]
        public void do_nothing_if_the_tag_already_has_text()
        {
            new HtmlTag("div").Text("original").TextIfEmpty("different")
                .Text().ShouldBe("original");
        }

        [Fact]
        public void set_the_text_if_the_tag_text_is_initially_null()
        {
            new HtmlTag("div").TextIfEmpty("defaulted")
                .Text().ShouldBe("defaulted");
        }

    }
}