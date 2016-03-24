using FubuCore;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Web.UI;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
	[TestFixture]
	public class RegularExpressionModifierTester : ValidationElementModifierContext<RegularExpressionModifier>
	{
		public const string Expression = "[a-zA-Z0-9]+$";

		[Test]
		public void adds_the_rangelength_data_attribute_for_range_length_rule()
		{
			var theRequest = ElementRequest.For(new TargetWithRegex(), x => x.Value);
			var value = tagFor(theRequest).Data("regex").As<string>();

			value.ShouldBe(Expression);
		}

		[Test]
		public void no_rangelength_data_attribute_when_rule_does_not_exist()
		{
			var theRequest = ElementRequest.For(new TargetWithNoRegex(), x => x.Value);
			tagFor(theRequest).Data("rangelength").ShouldBeNull();
		}


		public class TargetWithRegex
		{
			[RegularExpression(Expression)]
			public string Value { get; set; }
		}

		public class TargetWithNoRegex
		{
			public string Value { get; set; }
		}
	}
}