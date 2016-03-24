using System.Linq;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Tests.Validation.Models;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Validation.Fields
{
    [TestFixture]
    public class GreaterThanZeroRuleTester
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
		public void uses_the_default_token()
		{
			theRule.Token.ShouldBe(ValidationKeys.GreaterThanZero);
		}

        [Test]
        public void should_register_message_if_value_is_less_than_zero()
        {
            theModel.GreaterThanZero = -1;
            theRule.ValidateProperty(theModel, x => x.GreaterThanZero)
                .MessagesFor<SimpleModel>(x => x.GreaterThanZero).Select(x => x.StringToken).ShouldHaveTheSameElementsAs(ValidationKeys.GreaterThanZero);
        }

        [Test]
        public void should_register_a_message_if_value_is_zero()
        {
            theModel.GreaterThanZero = 0;
            theRule.ValidateProperty(theModel, x => x.GreaterThanZero)
                .MessagesFor<SimpleModel>(x => x.GreaterThanZero)
                .Select(x => x.StringToken).ShouldHaveTheSameElementsAs(ValidationKeys.GreaterThanZero);
        }

        [Test]
        public void should_not_register_a_message_if_value_is_greater_than_zero()
        {
            theModel.GreaterThanZero = 10;
            theRule.ValidateProperty(theModel, x => x.GreaterThanZero)
                .MessagesFor<SimpleModel>(x => x.GreaterThanZero).Any().ShouldBeFalse();
        }
    }
}