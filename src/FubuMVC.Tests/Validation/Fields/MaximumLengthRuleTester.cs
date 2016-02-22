using System.Linq;
using FubuTestingSupport;
using FubuValidation.Fields;
using FubuValidation.Tests.Models;
using NUnit.Framework;

namespace FubuValidation.Tests.Fields
{
    [TestFixture]
    public class MaximumLengthRuleTester
    {
        private AddressModel theModel;

        [SetUp]
        public void SetUp()
        {
            theModel = new AddressModel();
        }

        private Notification theNotification
        {
            get
            {
                var scenario = ValidationScenario<AddressModel>.For(x =>
                {
                    x.Model = theModel;
                    x.FieldRule(new MaximumLengthRule(5));
                });

                return scenario.Notification;
            }
        }

		[Test]
		public void uses_the_default_token()
		{
			new MaximumLengthRule(0).Token.ShouldEqual(ValidationKeys.MaxLength);
		}

        [Test]
        public void should_not_register_message_if_value_is_null()
        {
            theModel.Address1 = null;
            theNotification.AllMessages.Any().ShouldBeFalse();
        }

        [Test]
        public void should_register_message_if_string_is_greater_than_limit()
        {
            theModel.Address1 = "Invalid property value";
            var theMessage = theNotification.MessagesFor<AddressModel>(x => x.Address1).Single();
            theMessage.GetMessage().ShouldEqual("Maximum length exceeded. Must be less than or equal to 5");
        }

        [Test]
        public void should_not_register_a_message_if_property_is_valid()
        {
            theModel.Address1 = "Valid";
            theNotification.AllMessages.Any().ShouldBeFalse();
        }
    }
}