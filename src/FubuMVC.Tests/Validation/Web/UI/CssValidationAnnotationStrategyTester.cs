using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web.UI;
using HtmlTags;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
    [TestFixture]
    public class CssValidationAnnotationStrategyTester
    {
        private CssValidationAnnotationStrategy theStrategy;
        private HtmlTag theTag;
        private ElementRequest theRequest;

        [SetUp]
        public void SetUp()
        {
            theStrategy = new CssValidationAnnotationStrategy();
            theTag = new HtmlTag("input");

            theRequest = ElementRequest.For<CssTarget>(x => x.Name);
            theRequest.ReplaceTag(theTag);
        }

        [Test]
        public void matches_required_rules()
        {
            theStrategy.Matches(new RequiredFieldRule()).ShouldBeTrue();
        }

        [Test]
        public void matches_greater_than_zero_rules()
        {
            theStrategy.Matches(new GreaterThanZeroRule()).ShouldBeTrue();
        }

        [Test]
        public void matches_greater_than_or_equal_tozero_rules()
        {
            theStrategy.Matches(new GreaterOrEqualToZeroRule()).ShouldBeTrue();
        }

        [Test]
        public void matches_email_rule()
        {
            theStrategy.Matches(new EmailFieldRule()).ShouldBeTrue();
        }

        [Test]
        public void does_not_match_others()
        {
            theStrategy.Matches(new MinimumLengthRule(5)).ShouldBeFalse();
        }

        [Test]
        public void adds_the_required_css_class()
        {
            theStrategy.Modify(theRequest, new RequiredFieldRule());
            theTag.HasClass("required").ShouldBeTrue();
        }

        [Test]
        public void adds_the_greater_than_zero_css_class()
        {
            theStrategy.Modify(theRequest, new GreaterThanZeroRule());
            theTag.HasClass("greater-than-zero").ShouldBeTrue();
        }

        [Test]
        public void adds_the_greater_or_equal_to_zero_css_class()
        {
            theStrategy.Modify(theRequest, new GreaterOrEqualToZeroRule());
            theTag.HasClass("greater-equal-zero").ShouldBeTrue();
        }

        [Test]
        public void adds_the_email_css_class()
        {
            theStrategy.Modify(theRequest, new EmailFieldRule());
            theTag.HasClass("email").ShouldBeTrue();
        }

        public class CssTarget
        {
            public string Name { get; set; }
        }
    }
}