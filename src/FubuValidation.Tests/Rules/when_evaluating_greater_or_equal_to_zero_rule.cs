using System.Reflection;
using FubuValidation.Rules;
using NUnit.Framework;

namespace FubuValidation.Tests.Rules
{
    [TestFixture]
    public class when_evaluating_greater_or_equal_to_zero_rule
    {
        private Notification _notification;
        private SimpleModel _model;
        private PropertyInfo _property;
        private GreaterOrEqualToZeroValidationRule _rule;

        [SetUp]
        public void BeforeEach()
        {
            _notification = new Notification();
            _model = new SimpleModel();
            _property = Property.From<SimpleModel>(m => m.GreaterOrEqualToZero);
            _rule = new GreaterOrEqualToZeroValidationRule(_property);
        }

        [Test]
        public void should_register_message_if_value_is_less_than_zero()
        {
            _model.GreaterOrEqualToZero = -1;
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor(_property)
                .Messages
                .ShouldHaveCount(1);
        }

        [Test]
        public void should_not_register_a_message_if_value_is_zero()
        {
            _model.GreaterOrEqualToZero = 0;
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor(_property)
                .Messages
                .ShouldBeEmpty();
        }

        [Test]
        public void should_not_register_a_message_if_value_is_greater_than_zero()
        {
            _model.GreaterOrEqualToZero = 10;
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor(_property)
                .Messages
                .ShouldBeEmpty();
        }
    }
}