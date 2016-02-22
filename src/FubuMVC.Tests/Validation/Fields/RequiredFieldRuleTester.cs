using System.Linq;
using FubuTestingSupport;
using FubuValidation.Fields;
using FubuValidation.Tests.Models;
using NUnit.Framework;

namespace FubuValidation.Tests.Fields
{
    [TestFixture]
    public class RequiredFieldRuleTester
    {
        private AddressModel theTarget;

        [SetUp]
        public void SetUp()
        {
            theTarget = new AddressModel();
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

		[Test]
		public void defaults_to_required_key()
		{
			new RequiredFieldRule().Token.ShouldEqual(ValidationKeys.Required);
		}

        [Test]
        public void no_message_if_property_is_valid()
        {
            theTarget.Address1 = "1234 Test Lane";
            theNotification.MessagesFor<AddressModel>(x => x.Address1).Any().ShouldBeFalse();
        }

        [Test]
        public void registers_message_if_string_value_is_empty()
        {
            theTarget.Address1 = "";
            var messages = theNotification.MessagesFor<AddressModel>(x => x.Address1);
            messages.Single().StringToken.ShouldEqual(ValidationKeys.Required);
        }

        [Test]
        public void registers_message_if_value_is_null()
        {
            theTarget.Address1 = null;
            theNotification.MessagesFor<AddressModel>(x => x.Address1).Single().StringToken.ShouldEqual(ValidationKeys.Required);
        }
    }
}