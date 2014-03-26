using System;
using FubuMVC.Core.UI.Elements;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class HtmlConventionRegistryTester : HtmlElementConventionsContext
    {
        [Test]
        public void basic_add_builder_1()
        {
            theRegistry.Displays.Add(new FakeDisplayBuilder());
            theTarget.Name = "Shiner";

            generator.DisplayFor(x => x.Name).ToString().ShouldEqual("<span class=\"fake\">Shiner</span>");
        }

        [Test]
        public void basic_add_builder_2()
        {
            theRegistry.Displays.BuilderPolicy<FakeDisplayBuilder>();
            theTarget.Name = "Shiner";

            generator.DisplayFor(x => x.Name).ToString().ShouldEqual("<span class=\"fake\">Shiner</span>");
        }

        [Test]
        public void basic_add_modifier_1()
        {
            theRegistry.Displays.Add(new FizzModifier());
            theRegistry.Displays.Add(new FakeDisplayBuilder());

            generator.DisplayFor(x => x.Name).HasClass("fizz").ShouldBeTrue();
        }

        [Test]
        public void basic_add_modifier_2()
        {
            theRegistry.Displays.Modifier<FizzModifier>();
            theRegistry.Displays.Add(new FakeDisplayBuilder());

            generator.DisplayFor(x => x.Name).HasClass("fizz").ShouldBeTrue(); 
        }

    }

    public class FakeDisplayBuilder : ElementTagBuilder
    {
        public override bool Matches(ElementRequest subject)
        {
            return true;
        }

        public override HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("span").Text(request.StringValue()).AddClass("fake");
        }
    }

    public class ConventionTarget
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public bool Passed { get; set; }
        public string BigName { get; set; }

        public DateTime Now { get; set; }
        public DateTime? NullableNow { get; set; }

        [Elements.FakeRequired]
        public string PropWithFakeReqired { get; set; }
        public string PropWithNoAttributes { get; set; }

        [Elements.FakeMaximumStringLength(25)]
        public string MaximumLengthProp { get; set; }
    }

    public class FizzModifier : IElementModifier
    {
        public bool Matches(ElementRequest token)
        {
            return true;
        }

        public void Modify(ElementRequest request)
        {
            request.CurrentTag.AddClass("fizz");
        }
    }

    
}