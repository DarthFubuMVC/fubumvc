using System.Linq;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Fields
{
    
    public class MinValueFieldRuleTester
    {
        private MinValueTarget theTarget = new MinValueTarget();

        private Notification theNotification
        {
            get
            {
                var scenario = ValidationScenario<MinValueTarget>.For(x =>
                {
                    x.Model = theTarget;
                    x.FieldRule(new MinValueFieldRule(10));
                });

                return scenario.Notification;
            }
        }

		[Fact]
		public void uses_the_default_token()
		{
			new MinValueFieldRule(0).Token.ShouldBe(ValidationKeys.MinValue);
		}

        [Fact]
        public void registers_a_message_when_the_value_is_less_than_min()
        {
            theTarget.Value = 6;
            var messages = theNotification.MessagesFor<MinValueTarget>(x => x.Value);
            messages.Single().StringToken.ShouldBe(ValidationKeys.MinValue);
        }

        [Fact]
        public void no_message_when_equal_to_min()
        {
            theTarget.Value = 10;
            theNotification.MessagesFor<MinValueTarget>(x => x.Value).Any().ShouldBeFalse();
        }

        [Fact]
        public void no_message_when_greater_than_min()
        {
            theTarget.Value = 11;
            theNotification.MessagesFor<MinValueTarget>(x => x.Value).Any().ShouldBeFalse();
        }

        [Fact]
        public void renders_the_substitutions()
        {
            theTarget.Value = 7;
            var message = theNotification.MessagesFor<MinValueTarget>(x => x.Value).Single();
            message.GetMessage().ShouldBe("Value must be greater than or equal to 10");
        }


        public class MinValueTarget
        {
            public int Value { get; set; }
        }
    }
}