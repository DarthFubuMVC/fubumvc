using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuLocalization;
using System.Linq;

namespace FubuValidation
{
    public class Notification
    {
        // TODO -- make this a list!!
        private readonly IList<NotificationMessage> _messages = new List<NotificationMessage>();

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
            get { return _messages; }
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



    }
}