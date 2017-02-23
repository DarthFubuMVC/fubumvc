using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Validation;
using FubuMVC.Tests.Validation.Models;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation
{
    
    public class NotificationTester
    {
        [Fact]
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

        [Fact]
        public void valid_should_return_valid_notification()
        {
            var notification = new Notification();
            notification
                .IsValid()
                .ShouldBeTrue();
        }

        [Fact]
        public void should_be_invalid_if_any_messages_are_registered()
        {
            var notification = new Notification();
            notification.RegisterMessage<EntityToValidate>(e => e.Something, StringToken.FromKeyString("test"));

            notification
                .IsValid()
                .ShouldBeFalse();
        }

        [Fact]
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

        [Fact]
        public void to_validation_error_simple()
        {
            var notification = new Notification();
            notification.RegisterMessage<EntityToValidate>(e => e.Something, StringToken.FromKeyString("test1", "test1"));
            notification.RegisterMessage<EntityToValidate>(e => e.Something, StringToken.FromKeyString("test2", "test2"));
            notification.RegisterMessage<EntityToValidate>(e => e.Something, StringToken.FromKeyString("test3", "test3"));

            var errors = notification.ToValidationErrors();
            CoreExtensions.Count(errors).ShouldBe(3);
            errors.First().message.ShouldBe("test1");
            errors.First().field.ShouldBe("Something");

        }

        [Fact]
        public void to_validation_error_when_an_error_is_registered_without_an_accessor()
        {
            var notification = new Notification();
            notification.RegisterMessage(StringToken.FromKeyString("test1", "test1"));

            var error = notification.ToValidationErrors().Single();
            error.message.ShouldBe("test1");
            error.field.ShouldBeEmpty();
        }

        [Fact]
        public void to_validation_error_with_localization()
        {
            LocalizationManager.Stub();

            var notification = new Notification();
            notification.RegisterMessage<EntityToValidate>(e => e.Something, StringToken.FromKeyString("test1", "test1"));

            var errors = notification.ToValidationErrors();
            errors.First().label.ShouldBe("en-US_Something");
        }

        [Fact]
        public void to_validation_error_if_multiple_accessors_match_a_message()
        {
            var notification = new Notification();
            var token = StringToken.FromKeyString("test1", "test1");
            var message = new NotificationMessage(token);
            message.AddAccessor(ReflectionHelper.GetAccessor<EntityToValidate>(x => x.Something));
            message.AddAccessor(ReflectionHelper.GetAccessor<EntityToValidate>(x => x.Else));

            notification.RegisterMessage(message);

            var errors = notification.ToValidationErrors();
            errors.Length.ShouldBe(2);

            errors.Each(x => x.message.ShouldBe("test1"));

            errors.First().field.ShouldBe("Something");
            errors.Last().field.ShouldBe("Else");
        }

        [Fact]
        public void add_child()
        {
            var child = new Notification();
            child.RegisterMessage<ContactModel>(x => x.FirstName, ValidationKeys.Required);
            child.RegisterMessage<ContactModel>(x => x.LastName, ValidationKeys.Required);

            var notification = new Notification(typeof(CompositeModel));
            var property = ReflectionHelper.GetAccessor<CompositeModel>(x => x.Contact);

            notification.AddChild(property, child);

            notification.MessagesFor<CompositeModel>(x => x.Contact.FirstName).Single().StringToken.ShouldBe(ValidationKeys.Required);
            notification.MessagesFor<CompositeModel>(x => x.Contact.LastName).Single().StringToken.ShouldBe(ValidationKeys.Required);
        }

        [Fact]
        public void registering_a_message_adds_the_default_field_template_value()
        {
            var accessor = ReflectionHelper.GetAccessor<EntityToValidate>(x => x.Something);
            var notification = new Notification();
            notification.RegisterMessage(accessor, new NotificationMessage(StringToken.FromKeyString("Test", "{field}")));

            var message = notification.MessagesFor(accessor).Single();
            message.GetMessage().ShouldBe(LocalizationManager.GetText(accessor.InnerProperty));
        }

        [Fact]
        public void registering_a_message_with_a_lambda_adds_the_message_to_the_notification()
        {
            Expression<Func<EntityToValidate, object>> somethingGetter = x => x.Something;
            var accessor = ReflectionHelper.GetAccessor(somethingGetter);
            var notification = new Notification();
            notification.RegisterMessage(somethingGetter, "a value");

            var message = notification.MessagesFor(accessor).Single();
            message.GetMessage().ShouldBe("a value");
        }

        #region Nested Type: EntityToValidate
        public class EntityToValidate
        {
            public string Something { get; set; }
            public string Else { get; set; }
            public EntityToValidate Child { get; set; }
        }
        #endregion
    }
}