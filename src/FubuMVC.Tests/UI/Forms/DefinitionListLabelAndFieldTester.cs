using FubuMVC.UI.Forms;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Forms
{
    [TestFixture]
    public class DefinitionListLabelAndFieldTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void place_label_tag()
        {
            var layout = new DefinitionListLabelAndField();
            var label = new HtmlTag("span").Text("some text");

            layout.LabelTag = label;
            layout.LabelTag.ShouldBeTheSameAs(label);
        }

        [Test]
        public void place_body_tag()
        {
            var layout = new DefinitionListLabelAndField();
            var label = new HtmlTag("span").Text("some text");

            layout.BodyTag = label;
            layout.BodyTag.ShouldBeTheSameAs(label);
        }

        [Test]
        public void write_to_string()
        {
            var layout = new DefinitionListLabelAndField();
            var label = new HtmlTag("span").Text("some text");
            layout.LabelTag = label;

            var display = new TextboxTag().Attr("value", "something");
            layout.BodyTag = display;

            var html = layout.ToString();
        
            html.ShouldContain(label.ToString());
            html.ShouldContain(display.ToString());
        }

        [Test]
        public void replace_the_label()
        {
            var layout = new DefinitionListLabelAndField();
            var label = new HtmlTag("span").Text("some text");
            layout.LabelTag = label;

            var display = new TextboxTag().Attr("value", "something");
            layout.LabelTag = display;

            layout.LabelTag.ShouldBeTheSameAs(display);
        }

        [Test]
        public void wrap_body()
        {
            var layout = new DefinitionListLabelAndField();
            var display = new TextboxTag().Attr("value", "something");
            layout.BodyTag = display;

            var wrapper = new HtmlTag("strong");
            layout.WrapBody(wrapper);

            layout.BodyTag.ShouldBeTheSameAs(display);
            wrapper.Children.ShouldHaveTheSameElementsAs(display);

            layout.ToString().ShouldContain(wrapper.ToString());
        }
    }
}