using NUnit.Framework;

namespace FubuValidation.Tests
{
    [TestFixture]
    public class NotificationTester
    {
        [Test]
        public void should_return_child()
        {
            var notification = new Notification();
            var child = new Notification();
            notification.AddChild<EntityToValidate>(e => e.Something, child);

            Assert.AreSame(child, notification.GetChild<EntityToValidate>(e => e.Something));
        }

        [Test]
        public void should_return_valid_if_child_does_not_exist()
        {
            var notification = new Notification();
            Notification child = notification.GetChild<EntityToValidate>(e => e.Something);

            child.IsValid().ShouldBeTrue();
        }

        [Test]
        public void should_ignore_duplicates()
        {
            var notification = new Notification();
            notification.RegisterMessage<EntityToValidate>(e => e.Something, "message");
            notification.RegisterMessage<EntityToValidate>(e => e.Something, "message");
            notification.RegisterMessage<EntityToValidate>(e => e.Something, "message");
            notification.RegisterMessage<EntityToValidate>(e => e.Something, "message");
            notification.RegisterMessage<EntityToValidate>(e => e.Something, "message");

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
            notification.RegisterMessage<EntityToValidate>(e => e.Something, "message");

            notification
                .IsValid()
                .ShouldBeFalse();
        }

        [Test]
        public void should_return_registered_messages()
        {
            var notification = new Notification();
            notification.RegisterMessage<EntityToValidate>(e => e.Something, "message1");
            notification.RegisterMessage<EntityToValidate>(e => e.Something, "message2");
            notification.RegisterMessage<EntityToValidate>(e => e.Something, "message3");


            notification
                .MessagesFor<EntityToValidate>(e => e.Something)
                .Messages
                .ShouldHaveCount(3);
        }

        [Test]
        public void should_be_invalid_if_children_are_invalid()
        {
            var notification = new Notification();
            var childNotification = new Notification();

            childNotification.RegisterMessage<EntityToValidate>(e => e.Something, "invalid");
            notification.AddChild<EntityToValidate>(e => e.Child, childNotification);

            notification
                .IsValid()
                .ShouldBeFalse();
        }

        [Test]
        public void should_recursively_flatten__child_notifications()
        {
            Notification notification = new Notification();
            notification.RegisterMessage<EntityToValidate>(e => e.Child, "invalid");

            Notification child = new Notification();
            child.RegisterMessage<EntityToValidate>(e => e.Child, "invalid");

            notification.AddChild<EntityToValidate>(e => e.Child, child);

            Notification grandchild = new Notification();
            grandchild.RegisterMessage<EntityToValidate>(e => e.Something, "invalid");

            child.AddChild<EntityToValidate>(e => e.Child, grandchild);

            Notification flat = notification.Flatten();
            flat
                .AllMessages
                .Length
                .ShouldEqual(3);
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