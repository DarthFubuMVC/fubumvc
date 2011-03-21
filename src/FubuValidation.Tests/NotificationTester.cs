using FubuCore.Reflection;
using FubuLocalization;
using FubuValidation.Tests.Models;
using NUnit.Framework;
using System.Linq;

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

        [Test]
        public void add_child()
        {
            var child = new Notification();
            child.RegisterMessage<ContactModel>(x => x.FirstName, ValidationKeys.REQUIRED);
            child.RegisterMessage<ContactModel>(x => x.LastName, ValidationKeys.REQUIRED);

            var notification = new Notification(typeof (CompositeModel));
            var property = ReflectionHelper.GetAccessor<CompositeModel>(x => x.Contact);

            notification.AddChild(property, child);

            notification.MessagesFor<CompositeModel>(x => x.Contact.FirstName).Single().StringToken.ShouldEqual(ValidationKeys.REQUIRED);
            notification.MessagesFor<CompositeModel>(x => x.Contact.LastName).Single().StringToken.ShouldEqual(ValidationKeys.REQUIRED);
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