using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Forms;
using FubuMVC.Core.UI.Tags;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.UI.Forms
{
    [TestFixture]
    public class SimpleFormTester
    {
        private DefinitionListLabelAndField theRenderedFormLine;

        [Test]
        public void a_basic_test()
        {
            var registry = new FubuRegistry(x =>
            {
                x.HtmlConvention<DefaultHtmlConventions>();
            });
            var container = new Container();

            FubuApplication.For(registry).StructureMap(container).Bootstrap();

            var tags = container.GetInstance<TagGenerator<AddressViewModel>>();
            tags.Model = new AddressViewModel()
                             {
                                 ShouldShow = true
                             };
          
            var form = new SimpleForm<AddressViewModel, DefinitionListLabelAndField>(tags);
            theRenderedFormLine = form.Display(p => p.ShouldShow);
        
            var xx = theRenderedFormLine.Render();
            
            xx.ShouldContain(tags.LabelFor(x=>x.ShouldShow).ToString());
            xx.ShouldContain(tags.DisplayFor(x=>x.ShouldShow).ToString());
        }
    }

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

            var html = layout.Render();
        
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
    }
}