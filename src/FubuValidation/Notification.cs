using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuLocalization;
using System.Linq;

namespace FubuValidation
{
    public class Notification
    {
        private readonly Cache<Accessor, IList<NotificationMessage>> _messages = new Cache<Accessor, IList<NotificationMessage>>(a => new List<NotificationMessage>());

        public Notification()
        {
        }

        public Notification(Type targetType)
            : this()
        {
            TargetType = targetType;
        }

        public Type TargetType { get; private set; }

        public IEnumerable<NotificationMessage> AllMessages
        {
            get { return _messages.GetAll().SelectMany(x => x); }
        }

        public NotificationMessage RegisterMessage<T>(Expression<Func<T, object>> property, StringToken message)
        {
            return RegisterMessage(property.ToAccessor(), message);
        }

        public NotificationMessage RegisterMessage(Accessor accessor, StringToken notificationMessage)
        {
            var message = new NotificationMessage(notificationMessage);
            RegisterMessage(accessor, message);

            return message;
        }

        public void RegisterMessage(Accessor accessor, NotificationMessage notificationMessage)
        {
            notificationMessage.AddAccessor(accessor);
            _messages[accessor].Fill(notificationMessage);
        }

        public IEnumerable<NotificationMessage> MessagesFor(Accessor accessor)
        {
            return _messages[accessor];
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


    }
}