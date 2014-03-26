using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class configuring_conventions_by_profile : HtmlElementConventionsContext
    {
        [Test]
        public void use_the_correct_builder_by_profile()
        {
            theRegistry.Profile("important", x =>
            {
                x.Displays.Always.BuildBy(req => new HtmlTag("b").Text(req.StringValue()));
            });

            generator.DisplayFor(x => x.Name).TagName().ShouldEqual("span");
            generator.DisplayFor(x => x.Name, "important").TagName().ShouldEqual("b");
        }

        [Test]
        public void falls_through_from_one_profile_to_default_if_no_builder_matches()
        {
            theRegistry.Profile("important", x =>
            {
                x.Displays.IfPropertyHasAttribute<Elements.FakeRequiredAttribute>().BuildBy(req => new HtmlTag("b").Text(req.StringValue()));
            });

            generator.DisplayFor(x => x.PropWithFakeReqired, "important").TagName().ShouldEqual("b"); // should be caught by specific profile

            // fall through to the default profile to find a builder
            generator.DisplayFor(x => x.PropWithNoAttributes, "important").TagName().ShouldEqual("span"); 
        }

        [Test]
        public void modifiers_in_default_always_apply()
        {
            theRegistry.Displays.AddClassForAttribute<Elements.FakeRequiredAttribute>("required");

            generator.DisplayFor(x => x.PropWithFakeReqired, "important").HasClass("required").ShouldBeTrue();
        }


    }
}