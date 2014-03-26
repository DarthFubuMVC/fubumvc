using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class inline_definition_of_conditional_modifications_and_builders : HtmlElementConventionsContext
    {
        [Test]
        public void generic_filter_on_element_request()
        {
            theRegistry.Displays.If(req => req.Accessor.Name == "BigName").AddClass("buzz");

            generator.DisplayFor(x => x.Name).HasClass("buzz").ShouldBeFalse();
            generator.DisplayFor(x => x.BigName).HasClass("buzz").ShouldBeTrue();
        }

        [Test]
        public void filter_by_property_type()
        {
            theRegistry.Displays.IfPropertyIs<string>().AddClass("buzz");

            generator.DisplayFor(x => x.Passed).HasClass("buzz").ShouldBeFalse();

            generator.DisplayFor(x => x.BigName).HasClass("buzz").ShouldBeTrue();
            generator.DisplayFor(x => x.Name).HasClass("buzz").ShouldBeTrue();
        }

        [Test]
        public void filter_by_property_type_filter()
        {
            theRegistry.Displays.IfPropertyTypeIs(type => type.IsDateTime()).AddClass("date");

            generator.DisplayFor(x => x.Now).HasClass("date").ShouldBeTrue();
            generator.DisplayFor(x => x.NullableNow).HasClass("date").ShouldBeTrue();
            
            generator.DisplayFor(x => x.Name).HasClass("date").ShouldBeFalse();

            
        }
    }
}