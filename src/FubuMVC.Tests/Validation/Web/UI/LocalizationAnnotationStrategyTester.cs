using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web.UI;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
	[TestFixture]
	public class LocalizationAnnotationStrategyTester
	{
		private LocalizationAnnotationStrategy theStrategy;

		[SetUp]
		public void SetUp()
		{
			theStrategy = new LocalizationAnnotationStrategy();
		}

		[Test]
		public void matches_when_token_is_not_null()
		{
			var theRule = new RequiredFieldRule();
			theStrategy.Matches(theRule).ShouldBeTrue();
		}

		[Test]
		public void does_not_match_conditional_field_rules()
		{
			var theRule = new ConditionalFieldRule<AjaxTarget>(new IsValid(), new RequiredFieldRule());
			theStrategy.Matches(theRule).ShouldBeFalse();
		}

		[Test]
		public void no_match_when_token_is_null()
		{
			var theRule = new RequiredFieldRule();
			theRule.Token = null;
			theStrategy.Matches(theRule).ShouldBeFalse();
		}
	}
}