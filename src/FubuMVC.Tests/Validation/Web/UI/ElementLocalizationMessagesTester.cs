using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web.UI;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
	
	public class ElementLocalizationMessagesTester
	{
		private ElementLocalizationMessages theMessages = new ElementLocalizationMessages();

		[Fact]
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