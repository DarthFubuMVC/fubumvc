using System.Linq;
using FubuTestingSupport;
using FubuValidation.Fields;
using NUnit.Framework;

namespace FubuValidation.Tests.Fields
{
    [TestFixture]
    public class RangeLengthFieldRuleTester
    {
        private RangeLengthTarget theTarget;

        [SetUp]
        public void SetUp()
        {
            theTarget = new RangeLengthTarget();
        }

        private Notification theNotification
        {
            get
            {
                var scenario = ValidationScenario<RangeLengthTarget>.For(x =>
                {
                    x.Model = theTarget;
                    x.FieldRule(new RangeLengthFieldRule(5, 10));
                });

                return scenario.Notification;
            }
        }

		[Test]
		public void uses_the_default_token()
		{
			new RangeLengthFieldRule(0, 0).Token.ShouldEqual(ValidationKeys.RangeLength);
		}

        [Test]
        public void registers_a_message_when_the_length_is_less_than_min()
        {
            theTarget.Value = "1234";
            var messages = theNotification.MessagesFor<RangeLengthTarget>(x => x.Value);
            messages.Single().StringToken.ShouldEqual(ValidationKeys.RangeLength);
        }

        [Test]
        public void no_message_when_equal_to_min()
        {
            theTarget.Value = "12345";
            theNotification.MessagesFor<RangeLengthTarget>(x => x.Value).Any().ShouldBeFalse();
        }

        [Test]
        public void no_message_when_greater_than_min_and_less_than_max()
        {
            theTarget.Value = "123456789";
            theNotification.MessagesFor<RangeLengthTarget>(x => x.Value).Any().ShouldBeFalse();
        }

        [Test]
        public void no_message_when_equal_to_max()
        {
            theTarget.Value = "1234567890";
            theNotification.MessagesFor<RangeLengthTarget>(x => x.Value).Any().ShouldBeFalse();
        }

        [Test]
        public void registers_a_message_when_the_length_is_greater_than_max()
        {
            theTarget.Value = "12345678910";
            var messages = theNotification.MessagesFor<RangeLengthTarget>(x => x.Value);
            messages.Single().StringToken.ShouldEqual(ValidationKeys.RangeLength);
        }

        [Test]
        public void renders_the_substitutions()
        {
            theTarget.Value = "1234";
            var message = theNotification.MessagesFor<RangeLengthTarget>(x => x.Value).Single();
            message.GetMessage().ShouldEqual("Value must be between 5 and 10 characters");
        }


        public class RangeLengthTarget
        {
            public string Value { get; set; }
        }
    }
}