using FubuCore.Reflection;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web.Remote;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.Remote
{
    
    public class RemoteFieldValidationRuleFilterTester
    {
        private RemoteFieldValidationRuleFilter theFilter = new RemoteFieldValidationRuleFilter();

        [Fact]
        public void matches_rules_that_implment_IRemoteFieldValidationRule()
        {
            theFilter.Matches(new RemoteRuleStub()).ShouldBeTrue();
        }

        [Fact]
        public void does_not_match_rules_that_do_not_implement_IRemoteFieldValidationRule()
        {
            theFilter.Matches(new RequiredFieldRule()).ShouldBeFalse();
        }

        public class RemoteRuleStub : IRemoteFieldValidationRule
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