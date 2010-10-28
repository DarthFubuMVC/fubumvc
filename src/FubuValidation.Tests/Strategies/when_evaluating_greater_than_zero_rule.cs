using FubuCore;
using FubuCore.Reflection;
using FubuValidation.Strategies;
using NUnit.Framework;

namespace FubuValidation.Tests.Strategies
{
    [TestFixture]
    public class when_evaluating_greater_than_zero_rule
    {
        private Notification _notification;
        private SimpleModel _model;
        private GreaterThanZeroFieldStrategy _strategy;
        private FieldRule _rule;
        private Accessor _accessor;

        [SetUp]
        public void BeforeEach()
        {
            _notification = new Notification();
            _model = new SimpleModel();
            _strategy = new GreaterThanZeroFieldStrategy();
            _accessor = AccessorFactory.Create<SimpleModel>(m => m.GreaterThanZero);
            _rule = new FieldRule(_accessor, new TypeResolver(), _strategy);
        }

        [Test]
        public void should_register_message_if_value_is_less_than_zero()
        {
            _model.GreaterThanZero = -1;
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor(_accessor)
                .Messages
                .ShouldHaveCount(1);
        }

        [Test]
        public void should_register_a_message_if_value_is_zero()
        {
            _model.GreaterThanZero = 0;
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor(_accessor)
                .Messages
                .ShouldHaveCount(1);
        }

        [Test]
        public void should_not_register_a_message_if_value_is_greater_than_zero()
        {
            _model.GreaterThanZero = 10;
            _rule.Validate(_model, _notification);

            _notification
                .MessagesFor(_accessor)
                .Messages
                .ShouldBeEmpty();
        }
    }
}