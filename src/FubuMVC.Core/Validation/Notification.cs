using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Validation
{
    [Serializable]
    public class Notification
    {
        private readonly IList<NotificationMessage> _messages = new List<NotificationMessage>();
        public const string FIELD = "field";

        public Notification()
        {
        }

        public Notification(Type targetType)
            : this()
        {
            TargetType = targetType;
        }

        public ValidationError[] ToValidationErrors()
        {
            return AllMessages.SelectMany(x => x.ToValidationErrors()).ToArray();
        }

        public Type TargetType { get; private set; }

        public IEnumerable<NotificationMessage> AllMessages
        {
            get { return _messages; }
        }

        public NotificationMessage RegisterMessage<T>(Expression<Func<T, object>> property, StringToken message, params TemplateValue[] values)
        {
            return RegisterMessage(property.ToAccessor(), message, values);
        }

        public NotificationMessage RegisterMessage(Accessor accessor, StringToken notificationMessage, params TemplateValue[] values)
        {
            var message = new NotificationMessage(notificationMessage, values);
            RegisterMessage(accessor, message);

            return message;
        }

        public NotificationMessage RegisterMessage(PropertyInfo property, StringToken notificationMessage, params TemplateValue[] values)
        {
            return RegisterMessage(new SingleProperty(property), notificationMessage, values);
        }

        public void RegisterMessage<T>(Expression<Func<T, object>> property, string message)
        {
            var notificationMessage = new NotificationMessage(message);
            var accessor = property.ToAccessor();
            notificationMessage.AddAccessor(accessor);
            _messages.Fill(notificationMessage);
        }

        public void RegisterMessage(Accessor accessor, NotificationMessage notificationMessage)
        {
            notificationMessage.AddAccessor(accessor);
            notificationMessage.AddSubstitution(TemplateValue.For(FIELD, LocalizationManager.GetText(accessor.InnerProperty)));
            
            _messages.Fill(notificationMessage);
        }

        public void AddChild(Accessor accessor, Notification childNotification)
        {
            _messages.AddRange(childNotification.AllMessages.Select(x => x.Prepend(accessor)));
        }

        public IEnumerable<NotificationMessage> MessagesFor(Accessor accessor)
        {
            return _messages.Where(x => x.Accessors.Contains(accessor));
        }

        public IEnumerable<NotificationMessage> MessagesFor<T>(Expression<Func<T, object>> property)
        {
            return MessagesFor(property.ToAccessor());
        }

        public bool IsValid()
        {
            return !AllMessages.Any();
        }

        public static Notification Valid()
        {
            return new Notification();
        }

        public static Notification Invalid()
        {
            var notification = new Notification();
            notification.RegisterMessage(ValidationKeys.Required);

            return notification;
        }


        public void RegisterMessage(NotificationMessage message)
        {
            _messages.Add(message);
        }

        public NotificationMessage RegisterMessage(StringToken stringToken)
        {
            var message = new NotificationMessage(stringToken);
            RegisterMessage(message);

            return message;
        }

        public NotificationMessage RegisterMessage(string text)
        {
            var message = new NotificationMessage(text);
            RegisterMessage(message);

            return message;

        }
    }
}