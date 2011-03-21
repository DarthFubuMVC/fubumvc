using FubuLocalization;
using NUnit.Framework;

namespace FubuValidation.Tests
{
    [TestFixture]
    public class NotificationTester
    {
        [Test]
        public void should_ignore_duplicates()
        {
            var notification = new Notification();
            notification.RegisterMessage<EntityToValidate>(e => e.Something, StringToken.FromKeyString("test"));
            notification.RegisterMessage<EntityToValidate>(e => e.Something, StringToken.FromKeyString("test"));
            notification.RegisterMessage<EntityToValidate>(e => e.Something, StringToken.FromKeyString("test"));
            notification.RegisterMessage<EntityToValidate>(e => e.Something, StringToken.FromKeyString("test"));
            notification.RegisterMessage<EntityToValidate>(e => e.Something, StringToken.FromKeyString("test"));

            notification
                .AllMessages
                .ShouldHaveCount(1);
        }

        [Test]
        public void valid_should_return_valid_notification()
        {
            var notification = new Notification();
            notification
                .IsValid()
                .ShouldBeTrue();
        }

        [Test]
        public void should_be_invalid_if_any_messages_are_registered()
        {
            var notification = new Notification();
            notification.RegisterMessage<EntityToValidate>(e => e.Something, StringToken.FromKeyString("test"));

            notification
                .IsValid()
                .ShouldBeFalse();
        }

        [Test]
        public void should_return_registered_messages()
        {
            var notification = new Notification();
            notification.RegisterMessage<EntityToValidate>(e => e.Something, StringToken.FromKeyString("test1"));
            notification.RegisterMessage<EntityToValidate>(e => e.Something, StringToken.FromKeyString("test2"));
            notification.RegisterMessage<EntityToValidate>(e => e.Something, StringToken.FromKeyString("test3"));


            notification
                .MessagesFor<EntityToValidate>(e => e.Something)
                .ShouldHaveCount(3);
        }

        #region Nested Type: EntityToValidate
        public class EntityToValidate
        {
            public string Something { get; set; }
            public EntityToValidate Child { get; set; }
        }
        #endregion
    }
}