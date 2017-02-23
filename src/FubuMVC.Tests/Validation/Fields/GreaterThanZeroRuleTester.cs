using System.Linq;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Tests.Validation.Models;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Fields
{
    
    public class GreaterThanZeroRuleTester
    {
        private SimpleModel theModel;
        private GreaterThanZeroRule theRule;

        public GreaterThanZeroRuleTester()
        {
            theRule = new GreaterThanZeroRule();
            theModel = new SimpleModel();
        }

		[Fact]
		public void uses_the_default_token()
		{
			theRule.Token.ShouldBe(ValidationKeys.GreaterThanZero);
		}

        [Fact]
        public void should_register_message_if_value_is_less_than_zero()
        {
            theModel.GreaterThanZero = -1;
            theRule.ValidateProperty(theModel, x => x.GreaterThanZero)
                .MessagesFor<SimpleModel>(x => x.GreaterThanZero).Select(x => x.StringToken).ShouldHaveTheSameElementsAs(ValidationKeys.GreaterThanZero);
        }

        [Fact]
        public void should_register_a_message_if_value_is_zero()
        {
            theModel.GreaterThanZero = 0;
            theRule.ValidateProperty(theModel, x => x.GreaterThanZero)
                .MessagesFor<SimpleModel>(x => x.GreaterThanZero)
                .Select(x => x.StringToken).ShouldHaveTheSameElementsAs(ValidationKeys.GreaterThanZero);
        }

        [Fact]
        public void should_not_register_a_message_if_value_is_greater_than_zero()
        {
            theModel.GreaterThanZero = 10;
            theRule.ValidateProperty(theModel, x => x.GreaterThanZero)
                .MessagesFor<SimpleModel>(x => x.GreaterThanZero).Any().ShouldBeFalse();
        }
    }
}