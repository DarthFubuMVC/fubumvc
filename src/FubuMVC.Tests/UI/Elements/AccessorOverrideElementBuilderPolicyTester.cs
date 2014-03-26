using FubuCore.Reflection;
using FubuMVC.Core.UI.Elements;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Elements
{
    [TestFixture]
    public class AccessorOverrideElementBuilderPolicyTester
    {
        private AccessorRules theOverrides;
        private AccessorOverrideElementBuilderPolicy thePolicy;

        [SetUp]
        public void SetUp()
        {
            theOverrides = new AccessorRules();
            thePolicy = new AccessorOverrideElementBuilderPolicy(theOverrides, "C1", "P1");
        }

        [Test]
        public void matches_negative_with_no_overrides()
        {
            var request = ElementRequest.For<OverrideTarget>(x => x.Name);

            thePolicy.Matches(request).ShouldBeFalse();
        }

        [Test]
        public void matches_negative_with_no_overrides_for_this_accessor()
        {
            theOverrides.Add<OverrideTarget>(x => x.Age, new ElementTagOverride<SimpleBuilder>("C1", "P1"));

            var request = ElementRequest.For<OverrideTarget>(x => x.Name);

            thePolicy.Matches(request).ShouldBeFalse();
        }

        [Test]
        public void matches_negative_with_an_override_for_the_accessor_but_the_wrong_category()
        {
            theOverrides.Add<OverrideTarget>(x => x.Age, new ElementTagOverride<SimpleBuilder>("Wrong", "P1"));

            var request = ElementRequest.For<OverrideTarget>(x => x.Age);

            thePolicy.Matches(request).ShouldBeFalse();
        }

        [Test]
        public void matches_negative_with_an_override_for_the_accessor_but_the_wrong_profile()
        {
            theOverrides.Add<OverrideTarget>(x => x.Age, new ElementTagOverride<SimpleBuilder>("C1", "Wrong"));

            var request = ElementRequest.For<OverrideTarget>(x => x.Age);

            thePolicy.Matches(request).ShouldBeFalse();
        }

        [Test]
        public void the_stars_completely_align_and_we_have_a_positive_match()
        {
            theOverrides.Add<OverrideTarget>(x => x.Age, new ElementTagOverride<SimpleBuilder>("C1", "P1"));

            var request = ElementRequest.For<OverrideTarget>(x => x.Age);

            thePolicy.Matches(request).ShouldBeTrue();
        }

        [Test]
        public void get_the_builder()
        {
            theOverrides.Add<OverrideTarget>(x => x.Age, new ElementTagOverride<SimpleBuilder>("C1", "P1"));
            theOverrides.Add<OverrideTarget>(x => x.Name, new ElementTagOverride<ComplexBuilder>("C1", "P1"));

            var request = ElementRequest.For<OverrideTarget>(x => x.Age);
            var request2 = ElementRequest.For<OverrideTarget>(x => x.Name);

            thePolicy.BuilderFor(request).ShouldBeOfType<SimpleBuilder>();
            thePolicy.BuilderFor(request2).ShouldBeOfType<ComplexBuilder>();
        }
    }

    public class OverrideTarget
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class SimpleBuilder : IElementBuilder
    {
        public HtmlTag Build(ElementRequest request)
        {
            throw new System.NotImplementedException();
        }
    }

    public class ComplexBuilder : IElementBuilder
    {
        public HtmlTag Build(ElementRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}