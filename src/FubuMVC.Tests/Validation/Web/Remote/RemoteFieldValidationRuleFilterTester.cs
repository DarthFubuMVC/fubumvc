using FubuCore.Reflection;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web.Remote;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.Remote
{
    [TestFixture]
    public class RemoteFieldValidationRuleFilterTester
    {
        private RemoteFieldValidationRuleFilter theFilter;

        [SetUp]
        public void SetUp()
        {
            theFilter = new RemoteFieldValidationRuleFilter();
        }

        [Test]
        public void matches_rules_that_implment_IRemoteFieldValidationRule()
        {
            theFilter.Matches(new RemoteRuleStub()).ShouldBeTrue();
        }

        [Test]
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