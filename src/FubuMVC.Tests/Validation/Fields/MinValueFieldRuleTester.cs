using System.Linq;
using FubuTestingSupport;
using FubuValidation.Fields;
using NUnit.Framework;

namespace FubuValidation.Tests.Fields
{
    [TestFixture]
    public class MinValueFieldRuleTester
    {
        private MinValueTarget theTarget;

        [SetUp]
        public void SetUp()
        {
            theTarget = new MinValueTarget();
        }

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

		[Test]
		public void uses_the_default_token()
		{
			new MinValueFieldRule(0).Token.ShouldEqual(ValidationKeys.MinValue);
		}

        [Test]
        public void registers_a_message_when_the_value_is_less_than_min()
        {
            theTarget.Value = 6;
            var messages = theNotification.MessagesFor<MinValueTarget>(x => x.Value);
            messages.Single().StringToken.ShouldEqual(ValidationKeys.MinValue);
        }

        [Test]
        public void no_message_when_equal_to_min()
        {
            theTarget.Value = 10;
            theNotification.MessagesFor<MinValueTarget>(x => x.Value).Any().ShouldBeFalse();
        }

        [Test]
        public void no_message_when_greater_than_min()
        {
            theTarget.Value = 11;
            theNotification.MessagesFor<MinValueTarget>(x => x.Value).Any().ShouldBeFalse();
        }

        [Test]
        public void renders_the_substitutions()
        {
            theTarget.Value = 7;
            var message = theNotification.MessagesFor<MinValueTarget>(x => x.Value).Single();
            message.GetMessage().ShouldEqual("Value must be greater than or equal to 10");
        }


        public class MinValueTarget
        {
            public int Value { get; set; }
        }
    }
}