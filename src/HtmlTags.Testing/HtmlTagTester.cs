using System;
using System.Diagnostics;
using NUnit.Framework;

namespace HtmlTags.Testing
{
    [TestFixture]
    public class HtmlTagTester
    {
        [Test]
        public void I_just_want_to_generate_a_div_with_text_and_a_class()
        {
            HtmlTag tag = new HtmlTag("div").Text("my text").AddClass("collapsible");
            tag.Add("span").Text("inner");
            Debug.WriteLine(tag.ToString());
        }

        [Test]
        public void insert_before()
        {
            var tag = new HtmlTag("div");
            tag.Add("span");
            tag.InsertFirst(new HtmlTag("p"));

            tag.ToCompacted().ShouldEqual("<div><p></p><span></span></div>");
        }

        [Test]
        public void is_input_element()
        {
            var tag = new HtmlTag("span");

            tag.IsInputElement().ShouldBeFalse();
            tag.TagName("input").IsInputElement().ShouldBeTrue();
            tag.TagName("select").IsInputElement().ShouldBeTrue();
        }

        [Test]
        public void is_visible_set_to_false()
        {
            new HtmlTag("div").Visible(false).ToCompacted().ShouldEqual("");
        }

        [Test]
        public void is_visible_set_to_true_by_default()
        {
            var tag = new HtmlTag("div");

            tag.Visible().ShouldBeTrue();
            tag.ToCompacted().ShouldEqual("<div></div>");
        }

        [Test]
        public void prepend()
        {
            HtmlTag tag = new HtmlTag("div").Text("something");
            tag.Prepend("more in front ");
            tag.ToCompacted().ShouldEqual("<div>more in front something</div>");
        }

        [Test]
        public void render_a_single_attribute()
        {
            HtmlTag tag = new HtmlTag("table").Attr("cellPadding", 2);
            tag.ToCompacted().ShouldEqual("<table cellPadding=\"2\"></table>");
        }

        [Test]
        public void render_a_single_class_even_though_it_is_registered_more_than_once()
        {
            HtmlTag tag = new HtmlTag("div").Text("text");
            tag.AddClass("a");

            tag.ToCompacted().ShouldEqual("<div class=\"a\">text</div>");

            tag.AddClass("a");

            tag.ToCompacted().ShouldEqual("<div class=\"a\">text</div>");
        }

        [Test]
        public void render_id()
        {
            HtmlTag tag = new HtmlTag("div").Id("theDiv");
            tag.ToCompacted().ShouldEqual("<div id=\"theDiv\"></div>");
        }

        [Test]
        public void render_metadata()
        {
            HtmlTag tag = new HtmlTag("div").Text("text");
            tag.MetaData("a", 1);
            tag.MetaData("b", "b-value");

            tag.ToCompacted().ShouldEqual("<div class=\"{&quot;a&quot;:1,&quot;b&quot;:&quot;b-value&quot;}\">text</div>");

            // now with another class
            tag.AddClass("class1");

            tag.ToCompacted().ShouldEqual("<div class=\"class1 {&quot;a&quot;:1,&quot;b&quot;:&quot;b-value&quot;}\">text</div>");
        }

        [Test]
        public void render_multiple_attributes()
        {
            HtmlTag tag = new HtmlTag("table").Attr("cellPadding", "2").Attr("cellSpacing", "3");
            tag.ToCompacted().ShouldEqual("<table cellPadding=\"2\" cellSpacing=\"3\"></table>");
        }

        [Test]
        public void render_multiple_classes()
        {
            HtmlTag tag = new HtmlTag("div").Text("text");
            tag.AddClass("a");
            tag.AddClass("b");
            tag.AddClass("c");

            tag.ToCompacted().ShouldEqual("<div class=\"a b c\">text</div>");
        }

        [Test]
        public void render_multiple_classes_with_a_single_method_call()
        {
            HtmlTag tag = new HtmlTag("div").Text("text");
            tag.AddClasses("a", "b", "c");

            tag.ToCompacted().ShouldEqual("<div class=\"a b c\">text</div>");
        }

        [Test]
        public void do_not_allow_spaces_in_class_names()
        {
            HtmlTag tag = new HtmlTag("div").Text("text");
            typeof(ArgumentException).ShouldBeThrownBy(() =>
            {
                tag.AddClass("a b c");
            });
        }

        [Test]
        public void render_multiple_levels_of_nesting()
        {
            var tag = new HtmlTag("table");
            tag.Add("tbody/tr/td").Text("some text");

            tag.ToCompacted()
                .ShouldEqual("<table><tbody><tr><td>some text</td></tr></tbody></table>");
        }

        [Test]
        public void render_multiple_levels_of_nesting_2()
        {
            HtmlTag tag = new HtmlTag("html").Modify(x =>
            {
                x.Add("head", head =>
                {
                    head.Add("title").Text("The title");
                    head.Add("style").Text("the style");
                });

                x.Add("body/div").Text("inner text of div");
            });

            tag.ToCompacted().ShouldEqual(
                "<html><head><title>The title</title><style>the style</style></head><body><div>inner text of div</div></body></html>");
        }

        [Test]
        public void render_simple_tag()
        {
            var tag = new HtmlTag("p");
            tag.ToString().ShouldEqual("<p></p>");
        }

        [Test]
        public void render_simple_tag_with_inner_text()
        {
            HtmlTag tag = new HtmlTag("p").Text("some text");
            tag.ToString().ShouldEqual("<p>some text</p>");
        }

        [Test]
        public void render_tag_with_one_child()
        {
            HtmlTag tag = new HtmlTag("div").Child(new HtmlTag("span").Text("something"));
            tag.ToCompacted().ShouldEqual("<div><span>something</span></div>");
        }

        [Test]
        public void replace_a_single_attribute()
        {
            HtmlTag tag = new HtmlTag("table")
                .Attr("cellPadding", 2)
                .Attr("cellPadding", 5);
            tag.ToCompacted().ShouldEqual("<table cellPadding=\"5\"></table>");
        }

        [Test]
        public void the_inner_text_is_html_encoded()
        {
            var tag = new HtmlTag("div");
            tag.Text("<b>Hi</b>");

            tag.ToCompacted().ShouldEqual("<div>&lt;b&gt;Hi&lt;/b&gt;</div>");
        }

        [Test]
        public void write_styles()
        {
            new HtmlTag("div")
                .Style("padding-left", "20px")
                .Style("padding-right", "30px")
                .ToCompacted()
                .ShouldEqual("<div style=\"padding-left:20px;padding-right:30px\"></div>");
        }

        [Test]
        public void write_deep_object_in_metadata()
        {
            new HtmlTag("div").MetaData("listValue", new ListValue
            {
                Display = "a",
                Value = "1"
            }).ToCompacted().ShouldEqual("<div class=\"{&quot;listValue&quot;:{&quot;Display&quot;:&quot;a&quot;,&quot;Value&quot;:&quot;1&quot;}}\"></div>");
        }

        public class ListValue
        {
            public string Display { get; set; }
            public string Value { get; set; }
        }
    }
}