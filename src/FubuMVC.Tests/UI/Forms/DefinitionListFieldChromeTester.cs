using FubuMVC.Core.UI.Forms;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Forms
{
    [TestFixture]
    public class DefinitionListFieldChromeTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void place_body_tag()
        {
            var layout = new DefinitionListFieldChrome();
            HtmlTag label = new HtmlTag("span").Text("some text");

            layout.BodyTag = label;
            layout.BodyTag.ShouldBeTheSameAs(label);
        }

        [Test]
        public void place_label_tag()
        {
            var layout = new DefinitionListFieldChrome();
            HtmlTag label = new HtmlTag("span").Text("some text");

            layout.LabelTag = label;
            layout.LabelTag.ShouldBeTheSameAs(label);
        }

        [Test]
        public void replace_the_label()
        {
            var layout = new DefinitionListFieldChrome();
            HtmlTag label = new HtmlTag("span").Text("some text");
            layout.LabelTag = label;

            HtmlTag display = new TextboxTag().Attr("value", "something");
            layout.LabelTag = display;

            layout.LabelTag.ShouldBeTheSameAs(display);
        }

        [Test]
        public void write_to_string()
        {
            var layout = new DefinitionListFieldChrome();
            HtmlTag label = new HtmlTag("span").Text("some text");
            layout.LabelTag = label;

            HtmlTag display = new TextboxTag().Attr("value", "something");
            layout.BodyTag = display;

            string html = layout.Render();

            html.ShouldContain(label.ToString());
            html.ShouldContain(display.ToString());
        }
    }
}