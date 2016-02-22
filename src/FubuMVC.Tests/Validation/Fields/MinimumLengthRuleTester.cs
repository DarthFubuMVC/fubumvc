using System.Linq;
using FubuTestingSupport;
using FubuValidation.Fields;
using FubuValidation.Tests.Models;
using NUnit.Framework;

namespace FubuValidation.Tests.Fields
{
    [TestFixture]
    public class MinimumLengthRuleTester
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
                    x.FieldRule(new MinimumLengthRule(5));
                });

                return scenario.Notification;
            }
        }

		[Test]
		public void uses_the_default_token()
		{
			new MinimumLengthRule(0).Token.ShouldEqual(ValidationKeys.MinLength);
		}

        [Test]
        public void should_not_register_message_if_value_is_null()
        {
            theModel.Address1 = null;
            theNotification.AllMessages.Any().ShouldBeFalse();
        }

        [Test]
        public void should_register_message_if_string_is_less_than_limit()
        {
            theModel.Address1 = "Inva";
            var theMessage = theNotification.MessagesFor<AddressModel>(x => x.Address1).Single();
            theMessage.GetMessage().ShouldEqual("Minimum length not met. Must be greater than or equal to 5");
        }

        [Test]
        public void should_not_register_a_message_if_property_is_valid()
        {
            theModel.Address1 = "Valid";
            theNotification.AllMessages.Any().ShouldBeFalse();
        }
    }
}