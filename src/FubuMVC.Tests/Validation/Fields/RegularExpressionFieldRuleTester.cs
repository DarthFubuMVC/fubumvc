using System.Linq;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Fields
{
	
	public class RegularExpressionFieldRuleTester
	{
		private RegExTarget theTarget = new RegExTarget();

		private Notification theNotification
		{
			get
			{
				var scenario = ValidationScenario<RegExTarget>.For(x =>
				{
					x.Model = theTarget;
					x.FieldRule(new RegularExpressionFieldRule("[a-zA-Z0-9]+$"));
				});

				return scenario.Notification;
			}
		}

		[Fact]
		public void uses_the_default_token()
		{
			new RegularExpressionFieldRule("[a-zA-Z0-9]+$").Token.ShouldBe(ValidationKeys.RegEx);
		}

		[Fact]
		public void registers_a_message_when_the_expression_does_not_match()
		{
			theTarget.Value = "hello//";
			var messages = theNotification.MessagesFor<RegExTarget>(x => x.Value);
			messages.Single().StringToken.ShouldBe(ValidationKeys.RegEx);
		}

		[Fact]
		public void no_message_when_the_expression_matches()
		{
			theTarget.Value = "hello";
			theNotification.MessagesFor<RegExTarget>(x => x.Value).Any().ShouldBeFalse();
		}

		[Fact]
		public void no_message_when_the_value_is_empty()
		{
			theTarget.Value = "";
			theNotification.MessagesFor<RegExTarget>(x => x.Value).Any().ShouldBeFalse();
		}


		public class RegExTarget
		{
			public string Value { get; set; }
		}
	}
}