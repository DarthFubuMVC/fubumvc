using System.Linq;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Tests.Validation.Models;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Fields
{
    
    public class GreaterOrEqualToZeroRuleTester
    {
        private SimpleModel theModel;
        private GreaterOrEqualToZeroRule theRule;

        public GreaterOrEqualToZeroRuleTester()
        {
            LocalizationManager.Stub();

            theRule = new GreaterOrEqualToZeroRule();
            theModel = new SimpleModel();
        }

		[Fact]
		public void uses_default_token()
		{
			theRule.Token.ShouldBe(ValidationKeys.GreaterThanOrEqualToZero);
		}

        [Fact]
        public void should_register_message_if_value_is_less_than_zero()
        {
            theModel.GreaterOrEqualToZero = -1;
            theRule.ValidateProperty(theModel, x => x.GreaterOrEqualToZero)
                .MessagesFor<SimpleModel>(x => x.GreaterOrEqualToZero).Select(x => x.StringToken)
                .ShouldHaveTheSameElementsAs(ValidationKeys.GreaterThanOrEqualToZero);
        }

        [Fact]
        public void should_not_register_a_message_if_value_is_zero()
        {
            theModel.GreaterOrEqualToZero = 0;
            theRule.ValidateProperty(theModel, x => x.GreaterOrEqualToZero)
                .MessagesFor<SimpleModel>(x => x.GreaterOrEqualToZero).Any().ShouldBeFalse();
        }

        [Fact]
        public void should_not_register_a_message_if_value_is_greater_than_zero()
        {
            theModel.GreaterOrEqualToZero = 10;
            theRule.ValidateProperty(theModel, x => x.GreaterOrEqualToZero)
                .MessagesFor<SimpleModel>(x => x.GreaterOrEqualToZero).Any().ShouldBeFalse();
        }
    }
}