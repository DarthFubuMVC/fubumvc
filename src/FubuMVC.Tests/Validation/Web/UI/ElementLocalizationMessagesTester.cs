using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web.UI;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
	[TestFixture]
	public class ElementLocalizationMessagesTester
	{
		private ElementLocalizationMessages theMessages;

		[SetUp]
		public void SetUp()
		{
			theMessages = new ElementLocalizationMessages();
		}

		[Test]
		public void adds_the_rules()
		{
			theMessages.Add(new RequiredFieldRule());
			theMessages.Add(new EmailFieldRule());

			var messages = theMessages.Messages;

			messages["required"].ShouldBe(ValidationKeys.Required.ToString());
			messages["email"].ShouldBe(ValidationKeys.Email.ToString());
		}
	}
}