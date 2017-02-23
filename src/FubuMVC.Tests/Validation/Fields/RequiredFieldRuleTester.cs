using System.Linq;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Tests.Validation.Models;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Fields
{
    
    public class RequiredFieldRuleTester
    {
        private AddressModel theTarget = new AddressModel();

        public RequiredFieldRuleTester()
        {
            LocalizationManager.Stub("en-US");
        }

        private Notification theNotification
        {
            get
            {
                var scenario = ValidationScenario<AddressModel>.For(x =>
                {
                    x.Model = theTarget;
                    x.FieldRule(new RequiredFieldRule());
                });

                return scenario.Notification;
            }
        }

		[Fact]
		public void defaults_to_required_key()
		{
			new RequiredFieldRule().Token.ShouldBe(ValidationKeys.Required);
		}

        [Fact]
        public void no_message_if_property_is_valid()
        {
            theTarget.Address1 = "1234 Test Lane";
            theNotification.MessagesFor<AddressModel>(x => x.Address1).Any().ShouldBeFalse();
        }

        [Fact]
        public void registers_message_if_string_value_is_empty()
        {
            theTarget.Address1 = "";
            var messages = theNotification.MessagesFor<AddressModel>(x => x.Address1);
            messages.Single().StringToken.ShouldBe(ValidationKeys.Required);
        }

        [Fact]
        public void registers_message_if_value_is_null()
        {
            theTarget.Address1 = null;
            theNotification.MessagesFor<AddressModel>(x => x.Address1).Single().StringToken.ShouldBe(ValidationKeys.Required);
        }
    }
}