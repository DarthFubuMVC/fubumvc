using System.Linq;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Tests.Validation.Models;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Fields
{
    
    public class MinimumLengthRuleTester
    {
        public MinimumLengthRuleTester()
        {
            LocalizationManager.Stub("en-US");
        }

        private AddressModel theModel = new AddressModel();

        private Notification theNotification
        {
            get
            {
                var scenario = ValidationScenario<AddressModel>.For(x =>
                {
                    x.Model = theModel;
                    x.FieldRule(new MinimumLengthRule(5));
                });

                return scenario.Notification;
            }
        }

		[Fact]
		public void uses_the_default_token()
		{
			new MinimumLengthRule(0).Token.ShouldBe(ValidationKeys.MinLength);
		}

        [Fact]
        public void should_not_register_message_if_value_is_null()
        {
            theModel.Address1 = null;
            theNotification.AllMessages.Any().ShouldBeFalse();
        }

        [Fact]
        public void should_register_message_if_string_is_less_than_limit()
        {
            theModel.Address1 = "Inva";
            var theMessage = theNotification.MessagesFor<AddressModel>(x => x.Address1).Single();
            theMessage.GetMessage().ShouldBe("Minimum length not met. Must be greater than or equal to 5");
        }

        [Fact]
        public void should_not_register_a_message_if_property_is_valid()
        {
            theModel.Address1 = "Valid";
            theNotification.AllMessages.Any().ShouldBeFalse();
        }
    }
}