using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuValidation.Strategies;
using NUnit.Framework;

namespace FubuValidation.Tests.Strategies
{
    [TestFixture]
    public class when_validating_required_fields
    {
        private Accessor _accessor;
        private FieldRule _rule;
        private RequiredFieldStrategy _strategy;
        private Notification _notification;
        private AddressModel _model;

        [SetUp]
        public void BeforeEach()
        {
            _notification = new Notification();
            _model = new AddressModel();
            _accessor = AccessorFactory.Create<AddressModel>(m => m.Address1);
            _strategy = new RequiredFieldStrategy();
            _rule = new FieldRule(_accessor, new TypeResolver(), _strategy);
        }

        [Test]
        public void should_register_message_if_value_is_null()
        {
            _model.Address1 = null;
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor(_accessor)
                .Messages
                .ShouldHaveCount(1);
        }

        [Test]
        public void should_register_message_if_string_value_is_empty()
        {
            _model.Address1 = "";
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor(_accessor)
                .Messages
                .ShouldHaveCount(1);
        }

        [Test]
        public void should_not_register_a_message_if_property_is_valid()
        {
            _model.Address1 = "1234 Test Lane";
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor(_accessor)
                .Messages
                .ShouldBeEmpty();
        }

        [Test]
        public void should_register_field_variable_in_substitutions_if_property_is_invalid()
        {
            should_register_message_if_value_is_null();

            _notification
                .MessagesFor(_accessor)
                .Messages
                .First()
                .MessageSubstitutions
                .ShouldContain(pair => pair.Key == RequiredFieldStrategy.FIELD);
        }
    }
}