using FubuCore;
using FubuCore.Reflection;
using FubuValidation.Fields;
using FubuValidation.Tests.Models;
using NUnit.Framework;
using System.Linq;

namespace FubuValidation.Tests.Fields
{
    [TestFixture]
    public class when_evaluating_greater_than_zero_rule
    {
        private SimpleModel theModel;
        private GreaterThanZeroRule theRule;

        [SetUp]
        public void BeforeEach()
        {
            theRule = new GreaterThanZeroRule();
            theModel = new SimpleModel();

        }

        [Test]
        public void should_register_message_if_value_is_less_than_zero()
        {
            theModel.GreaterThanZero = -1;
            theRule.Validate(theModel, x => x.GreaterThanZero)
                .MessagesFor<SimpleModel>(x => x.GreaterThanZero).Select(x => x.StringToken).ShouldHaveTheSameElementsAs(ValidationKeys.GREATER_THAN_ZERO);
        }

        [Test]
        public void should_register_a_message_if_value_is_zero()
        {
            theModel.GreaterThanZero = 0;
            theRule.Validate(theModel, x => x.GreaterThanZero)
                .MessagesFor<SimpleModel>(x => x.GreaterThanZero)
                .Select(x => x.StringToken).ShouldHaveTheSameElementsAs(ValidationKeys.GREATER_THAN_ZERO);
        }

        [Test]
        public void should_not_register_a_message_if_value_is_greater_than_zero()
        {
            theModel.GreaterThanZero = 10;
            theRule.Validate(theModel, x => x.GreaterThanZero)
                .MessagesFor<SimpleModel>(x => x.GreaterThanZero).Any().ShouldBeFalse();
        }
    }
}