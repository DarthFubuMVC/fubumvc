using FubuCore.Reflection;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web.Remote;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.Remote
{
    
    public class RemoteRuleAttributeFilterTester
    {
        private RemoteRuleAttributeFilter theFilter = new RemoteRuleAttributeFilter();

        [Fact]
        public void matches_rules_with_the_remote_attribute()
        {
            theFilter.Matches(new RuleWithRemoteAttribute()).ShouldBeTrue();
        }

        [Fact]
        public void does_not_match_rules_without_the_rule_attribute()
        {
            theFilter.Matches(new RequiredFieldRule()).ShouldBeFalse();
        }

        [Remote]
        public class RuleWithRemoteAttribute : IFieldValidationRule
        {
	        public StringToken Token { get; set; }

			public ValidationMode Mode { get; set; }

	        public void Validate(Accessor accessor, ValidationContext context)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}