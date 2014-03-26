using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Elements.Builders;
using FubuTestingSupport;
using HtmlTags.Conventions;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Elements
{
    [TestFixture]
    public class ElementTagOverrideTester
    {
        [Test]
        public void category_and_profile_are_default_by_default()
        {
            var @override = new ElementTagOverride<CheckboxBuilder>(null, null);

            @override.Category.ShouldEqual(TagConstants.Default);
            @override.Profile.ShouldEqual(TagConstants.Default);
        }

        [Test]
        public void can_happily_build_the_element_builder()
        {
            var @override = new ElementTagOverride<CheckboxBuilder>(null, null);
            @override.Builder().ShouldBeOfType<CheckboxBuilder>();
        }

        [Test]
        public void non_null_category_and_profile()
        {
            var @override = new ElementTagOverride<CheckboxBuilder>("c1", "p1");
            @override.Category.ShouldEqual("c1");
            @override.Profile.ShouldEqual("p1");
        }
    }
}