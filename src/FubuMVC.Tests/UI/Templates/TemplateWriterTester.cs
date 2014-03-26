using System.Linq;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Templates;
using FubuTestingSupport;
using HtmlTags;
using HtmlTags.Conventions;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Templates
{
    [TestFixture]
    public class TemplateWriterTester
    {
        private TemplateWriter theTemplates;

        [SetUp]
        public void SetUp()
        {
            var library = new DefaultHtmlConventions().Library;

            var namingConventions = new DefaultElementNamingConvention();

            var activators = new ElementIdActivator(namingConventions);

            theTemplates = new TemplateWriter(new ActiveProfile(), library, new TagRequestBuilder(new []{activators}));
        }

        [Test]
        public void do_not_write_anything_if_there_are_no_templates()
        {
            theTemplates.WriteAll().WillBeRendered().ShouldBeFalse();
        }

        [Test]
        public void writing_flushes_so_that_templates_do_not_get_added_twice()
        {
            theTemplates.AddTemplate("1", "hello");
            theTemplates.AddTemplate("2", "bye");

            theTemplates.WriteAll().ToString().ShouldContain("hello");

            theTemplates.AddTemplate("3", "laters");

            var result = theTemplates.WriteAll().ToString();

            result.ShouldContain("laters");
            result.ShouldNotContain("hello");
            result.ShouldNotContain("bye");
        }

        [Test]
        public void write_label()
        {
            theTemplates.LabelFor<ConventionTarget>(x => x.Name);

            theTemplates.WriteAll().FirstChild().ToString()
                .ShouldEqual("<div data-subject=\"label-ConventionTarget-Name\"><label for=\"Name\">Name</label></div>");
        }

        [Test]
        public void write_display()
        {
            theTemplates.DisplayFor<ConventionTarget>(x => x.Name);

            theTemplates.WriteAll().FirstChild().ToString()
                .ShouldEqual("<div data-subject=\"display-ConventionTarget-Name\"><span data-fld=\"Name\">{{Name}}</span></div>");
        }

        [Test]
        public void write_input()
        {
            theTemplates.InputFor<ConventionTarget>(x => x.Name);

            theTemplates.WriteAll().FirstChild().ToString()
                .ShouldEqual("<div data-subject=\"editor-ConventionTarget-Name\"><input type=\"text\" name=\"Name\" value=\"{{Name}}\" data-fld=\"Name\" /></div>");
        }
    }

    [TestFixture]
    public class when_writing_templates
    {
        private TemplateWriter theTemplates;
        private HtmlTag templates;

        [SetUp]
        public void SetUp()
        {
            var library = new DefaultHtmlConventions().Library;

            theTemplates = new TemplateWriter(new ActiveProfile(), library, new TagRequestBuilder(new ITagRequestActivator[0]));

            theTemplates.AddTemplate("foo", new HtmlTag("span").MustacheText("foo"));
            theTemplates.AddTemplate("bar", "some {{bar}} text");

            templates = theTemplates.WriteAll();
        }

        [Test]
        public void outside_tag_should_have_the_templates_class()
        {
            templates.HasClass("templates");
        }

        [Test]
        public void outside_tag_is_not_visible()
        {
            templates.Style("display").ShouldEqual("none");
        }

        [Test]
        public void writes_template_tag_inside_a_holder_with_subject()
        {
            templates.FirstChild().Attr("data-subject").ShouldEqual("foo");
            templates.FirstChild().FirstChild().ToString().ShouldEqual("<span>{{foo}}</span>");
        }

        [Test]
        public void writes_html_in_a_literal_tag()
        {
            templates.Children.Last().FirstChild().ShouldBeOfType<LiteralTag>()
                .ToString().ShouldEqual("some {{bar}} text");
        }
    }
}