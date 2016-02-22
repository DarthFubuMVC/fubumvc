using System.Linq;
using FubuTestingSupport;
using FubuValidation.Fields;
using NUnit.Framework;

namespace FubuValidation.Tests.Fields
{
	[TestFixture]
	public class RegularExpressionFieldRuleTester
	{
		private RegExTarget theTarget;

		[SetUp]
		public void SetUp()
		{
			theTarget = new RegExTarget();
		}

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

		[Test]
		public void uses_the_default_token()
		{
			new RegularExpressionFieldRule("[a-zA-Z0-9]+$").Token.ShouldEqual(ValidationKeys.RegEx);
		}

		[Test]
		public void registers_a_message_when_the_expression_does_not_match()
		{
			theTarget.Value = "hello//";
			var messages = theNotification.MessagesFor<RegExTarget>(x => x.Value);
			messages.Single().StringToken.ShouldEqual(ValidationKeys.RegEx);
		}

		[Test]
		public void no_message_when_the_expression_matches()
		{
			theTarget.Value = "hello";
			theNotification.MessagesFor<RegExTarget>(x => x.Value).Any().ShouldBeFalse();
		}

		[Test]
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