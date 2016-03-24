using System.Linq;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Tests.Validation.Models;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Validation.Fields
{
    [TestFixture]
    public class GreaterOrEqualToZeroRuleTester
    {
        private SimpleModel theModel;
        private GreaterOrEqualToZeroRule theRule;

        [SetUp]
        public void BeforeEach()
        {
            theRule = new GreaterOrEqualToZeroRule();
            theModel = new SimpleModel();
        }

		[Test]
		public void uses_default_token()
		{
			theRule.Token.ShouldBe(ValidationKeys.GreaterThanOrEqualToZero);
		}

        [Test]
        public void should_register_message_if_value_is_less_than_zero()
        {
            theModel.GreaterOrEqualToZero = -1;
            theRule.ValidateProperty(theModel, x => x.GreaterOrEqualToZero)
                .MessagesFor<SimpleModel>(x => x.GreaterOrEqualToZero).Select(x => x.StringToken)
                .ShouldHaveTheSameElementsAs(ValidationKeys.GreaterThanOrEqualToZero);
        }

        [Test]
        public void should_not_register_a_message_if_value_is_zero()
        {
            theModel.GreaterOrEqualToZero = 0;
            theRule.ValidateProperty(theModel, x => x.GreaterOrEqualToZero)
                .MessagesFor<SimpleModel>(x => x.GreaterOrEqualToZero).Any().ShouldBeFalse();
        }

        [Test]
        public void should_not_register_a_message_if_value_is_greater_than_zero()
        {
            theModel.GreaterOrEqualToZero = 10;
            theRule.ValidateProperty(theModel, x => x.GreaterOrEqualToZero)
                .MessagesFor<SimpleModel>(x => x.GreaterOrEqualToZero).Any().ShouldBeFalse();
        }
    }
}