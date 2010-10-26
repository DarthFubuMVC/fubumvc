using System.Reflection;
using FubuValidation.Rules;
using NUnit.Framework;

namespace FubuValidation.Tests.Rules
{
    [TestFixture]
    public class when_evaluating_greater_than_zero_rule
    {
        private Notification _notification;
        private SimpleModel _model;
        private PropertyInfo _property;
        private GreaterThanZeroValidationRule _rule;

        [SetUp]
        public void BeforeEach()
        {
            _notification = new Notification();
            _model = new SimpleModel();
            _property = Property.From<SimpleModel>(m => m.GreaterThanZero);
            _rule = new GreaterThanZeroValidationRule(_property);
        }

        [Test]
        public void should_register_message_if_value_is_less_than_zero()
        {
            _model.GreaterThanZero = -1;
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor(_property)
                .Messages
                .ShouldHaveCount(1);
        }

        [Test]
        public void should_register_a_message_if_value_is_zero()
        {
            _model.GreaterOrEqualToZero = 0;
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor(_property)
                .Messages
                .ShouldHaveCount(1);
        }

        [Test]
        public void should_not_register_a_message_if_value_is_greater_than_zero()
        {
            _model.GreaterThanZero = 10;
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor(_property)
                .Messages
                .ShouldBeEmpty();
        }
    }
}