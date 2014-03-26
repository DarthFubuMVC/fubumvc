using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class inline_definition_of_html_conventions_using_attributes : HtmlElementConventionsContext
    {
        [Test]
        public void use_attribute_as_a_filter()
        {
            theRegistry.Displays.IfPropertyHasAttribute<Elements.FakeRequiredAttribute>().BuildBy(req => new HtmlTag("b").Text(req.StringValue()));

            theTarget.PropWithFakeReqired = "some text";

            generator.DisplayFor(x => x.PropWithFakeReqired).ToString().ShouldEqual("<b>some text</b>");

            generator.DisplayFor(x => x.PropWithNoAttributes).TagName().ShouldEqual("span"); // default behavior
        }

        [Test]
        public void add_a_css_class_if_a_prop_has_an_attribute()
        {
            theRegistry.Displays.AddClassForAttribute<Elements.FakeRequiredAttribute>("required");

            generator.DisplayFor(x => x.PropWithFakeReqired).HasClass("required").ShouldBeTrue();
            generator.DisplayFor(x => x.PropWithNoAttributes).HasClass("required").ShouldBeFalse();
        }

        [Test]
        public void modify_for_attribute()
        {
            theRegistry.Editors.ModifyForAttribute<Elements.FakeMaximumStringLength>((tag, att) => tag.Attr("data-max-length", att.MaxLength));

            generator.InputFor(x => x.MaximumLengthProp).Attr("data-max-length").ShouldEqual("25");

            generator.InputFor(x => x.PropWithNoAttributes).HasAttr("data-max-length").ShouldBeFalse();
        }

        [Test]
        public void modify_for_attribute_when_you_do_not_care_about_the_attribute()
        {
            theRegistry.Editors.ModifyForAttribute<Elements.FakeRequiredAttribute>(tag => tag.AddClass("required"));

            generator.InputFor(x => x.PropWithFakeReqired).HasClass("required").ShouldBeTrue();
            generator.InputFor(x => x.PropWithNoAttributes).HasClass("required").ShouldBeFalse();
        }
    }
}