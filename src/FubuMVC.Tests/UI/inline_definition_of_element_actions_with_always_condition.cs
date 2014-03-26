using System;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Elements.Builders;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class inline_definition_of_element_actions_with_always_condition : HtmlElementConventionsContext
    {
        [Test]
        public void always_build_by_condition()
        {
            theRegistry.Displays.Always.BuildBy(new FakeDisplayBuilder());

            generator.DisplayFor(x => x.Name).HasClass("fake").ShouldBeTrue();
        }

        [Test]
        public void always_build_by_condition_2()
        {
            theRegistry.Displays.Always.BuildBy<FakeDisplayBuilder>();

            generator.DisplayFor(x => x.Name).HasClass("fake").ShouldBeTrue();
        }

        [Test]
        public void always_build_by_inline()
        {
            theRegistry.Displays.Always.BuildBy(req => new HtmlTag("b").Text(req.StringValue()).AddClass(req.ElementId));

            theTarget.Name = "Jeremy";
            generator.DisplayFor(x => x.Name).ToString().ShouldEqual("<b class=\"Name\">Jeremy</b>");
        }

        [Test]
        public void always_modify()
        {
            theRegistry.Displays.Always.ModifyWith(new AddClassModifier("fizz"));

            generator.DisplayFor(x => x.Name).HasClass("fizz").ShouldBeTrue();
        }

        [Test]
        public void always_modify_2()
        {
            theRegistry.Displays.Always.ModifyWith<FizzClassModifier>();

            generator.DisplayFor(x => x.Name).HasClass("fizz").ShouldBeTrue();
        }

        [Test]
        public void always_modify_inline()
        {
            theRegistry.Displays.Always.ModifyWith(req => req.CurrentTag.AddClass("buzz"));

            generator.DisplayFor(x => x.Name).HasClass("buzz").ShouldBeTrue();
        }

        [Test]
        public void always_add_attribute_value()
        {
            theRegistry.Displays.Always.Attr("data-foo", "25");

            generator.DisplayFor(x => x.Name).Attr("data-foo").ShouldEqual("25");
        }

        [Test]
        public void always_add_class()
        {
            theRegistry.Displays.Always.AddClass("buzz");

            generator.DisplayFor(x => x.Name).HasClass("buzz").ShouldBeTrue();
        }
    }

    
    public class FizzClassModifier : IElementModifier
    {
        public bool Matches(ElementRequest token)
        {
            throw new NotImplementedException();
        }

        public void Modify(ElementRequest request)
        {
            request.CurrentTag.AddClass("fizz");
        }
    }


    public static class HtmlTagTestingExtensions
    {
        public static bool WasBuiltByFakeDisplayBuilder(HtmlTag tag)
        {
            return tag.HasClass("fake");
        }
    }
}