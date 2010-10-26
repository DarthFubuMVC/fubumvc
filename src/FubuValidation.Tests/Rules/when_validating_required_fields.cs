using System.Linq;
using FubuValidation.Rules;
using NUnit.Framework;

namespace FubuValidation.Tests.Rules
{
    [TestFixture]
    public class when_validating_required_fields
    {
        private Notification _notification;
        private AddressModel _model;

        [SetUp]
        public void BeforeEach()
        {
            _notification = new Notification();
            _model = new AddressModel();
        }

        [Test]
        public void should_register_message_if_value_is_null()
        {
            _model.Address1 = null;
            var rule = new RequiredValidationRule(Property.From<AddressModel>(m => m.Address1));
            rule.Validate(_model, _notification);

            _notification
                .MessagesFor<AddressModel>(m => m.Address1)
                .Messages
                .ShouldHaveCount(1);
        }

        [Test]
        public void should_register_message_if_string_value_is_empty()
        {
            _model.Address1 = "";
            var rule = new RequiredValidationRule(Property.From<AddressModel>(m => m.Address1));
            rule.Validate(_model, _notification);

            _notification
                .MessagesFor<AddressModel>(m => m.Address1)
                .Messages
                .ShouldHaveCount(1);
        }

        [Test]
        public void should_not_register_a_message_if_property_is_valid()
        {
            _model.Address1 = "1234 Test Lane";
            var rule = new RequiredValidationRule(Property.From<AddressModel>(m => m.Address1));
            rule.Validate(_model, _notification);

            _notification
                .MessagesFor<AddressModel>(m => m.Address1)
                .Messages
                .ShouldBeEmpty();
        }

        [Test]
        public void should_register_field_variable_in_substitutions_if_property_is_invalid()
        {
            should_register_message_if_value_is_null();

            _notification
                .MessagesFor<AddressModel>(m => m.Address1)
                .Messages
                .First()
                .MessageSubstitutions
                .ShouldContain(pair => pair.Key == RequiredValidationRule.FIELD);
        }
    }
}