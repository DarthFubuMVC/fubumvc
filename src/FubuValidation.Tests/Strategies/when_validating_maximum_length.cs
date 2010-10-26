using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuValidation.Strategies;
using NUnit.Framework;

namespace FubuValidation.Tests.Strategies
{
    [TestFixture]
    public class when_validating_maximum_length
    {
        private Notification _notification;
        private AddressModel _model;
        private MaximumStringLengthFieldStrategy _strategy;
        private FieldRule _rule;
        private Accessor _accessor;

        [SetUp]
        public void BeforeEach()
        {
            _notification = new Notification();
            _model = new AddressModel();
            _strategy = new MaximumStringLengthFieldStrategy() { Length = 10 };
            _accessor = AccessorFactory.Create<AddressModel>(m => m.Address1);
            _rule = new FieldRule(_accessor, new TypeResolver(), _strategy);
        }

        [Test]
        public void should_not_register_message_if_value_is_null()
        {
            _model.Address1 = null;
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor(_accessor)
                .Messages
                .ShouldBeEmpty();
        }

        [Test]
        public void should_register_message_if_string_is_greater_than_limit()
        {
            _model.Address1 = "Invalid property value";
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor(_accessor)
                .Messages
                .ShouldHaveCount(1);
        }

        [Test]
        public void should_not_register_a_message_if_property_is_valid()
        {
            _model.Address1 = "Valid";
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor(_accessor)
                .Messages
                .ShouldBeEmpty();
        }

        [Test]
        public void should_register_field_variable_in_substitutions_if_property_is_invalid()
        {
            should_register_message_if_string_is_greater_than_limit();

            _notification
                .MessagesFor(_accessor)
                .Messages
                .First()
                .MessageSubstitutions
                .ShouldContain(pair => pair.Key == MaximumStringLengthFieldStrategy.LENGTH && pair.Value == "10");
        }
    }
}