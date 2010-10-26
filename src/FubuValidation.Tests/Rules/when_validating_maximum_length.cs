using FubuValidation.Rules;
using NUnit.Framework;

namespace FubuValidation.Tests.Rules
{
    [TestFixture]
    public class when_validating_maximum_length
    {
        private Notification _notification;
        private AddressModel _model;
        private MaximumStringLengthValidationRule _rule;

        [SetUp]
        public void BeforeEach()
        {
            _notification = new Notification();
            _model = new AddressModel();
            _rule = new MaximumStringLengthValidationRule(Property.From<AddressModel>(m => m.Address1), 10);
        }

        [Test]
        public void should_not_register_message_if_value_is_null()
        {
            _model.Address1 = null;
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor<AddressModel>(m => m.Address1)
                .Messages
                .ShouldBeEmpty();
        }

        [Test]
        public void should_register_message_if_string_is_greater_than_limit()
        {
            _model.Address1 = "Invalid property value";
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor<AddressModel>(m => m.Address1)
                .Messages
                .ShouldHaveCount(1);
        }

        [Test]
        public void should_not_register_a_message_if_property_is_valid()
        {
            _model.Address1 = "Valid";
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor<AddressModel>(m => m.Address1)
                .Messages
                .ShouldBeEmpty();
        }
    }
}