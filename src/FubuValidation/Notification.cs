using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuLocalization;

namespace FubuValidation
{
    public class Notification
    {
        private readonly List<NotificationMessage> _messages = new List<NotificationMessage>();
        private readonly Cache<PropertyInfo, MessageBag> _bags;
        private readonly Dictionary<PropertyInfo, Notification> _children = new Dictionary<PropertyInfo, Notification>();

        public Notification()
        {
            _bags = new Cache<PropertyInfo, MessageBag>
                        {
                            OnMissing = (property => new MessageBag(property))
                        };
        }

        public Notification(Type targetType)
        {
            TargetType = targetType;
        }

        public Type TargetType { get; private set; }

        public NotificationMessage[] AllMessages { get { return _messages.ToArray(); } }

        public void Include(Notification notification)
        {
            _messages.AddRange(notification.AllMessages);
        }

        public NotificationMessage RegisterMessage<TARGET>(Expression<Func<TARGET, object>> property, string messageTemplate)
        {
            return RegisterMessage(ReflectionHelper.GetProperty(property), messageTemplate);
        }

        public NotificationMessage RegisterMessage(PropertyInfo property, string messageTemplate)
        {
            var notificationMessage = new NotificationMessage(StringToken.FromKeyString("key"));
            notificationMessage.AddProperty(property);

            if(!_messages.Contains(notificationMessage))
            {
                _messages.Add(notificationMessage);
                MessagesFor(property)
                    .Add(notificationMessage);
            }

            return notificationMessage;
        }

        public Notification GetChild(PropertyInfo property)
        {
            if(_children.ContainsKey(property))
            {
                return _children[property];
            }

            return Valid();
        }

        public Notification GetChild<TARGET>(Expression<Func<TARGET, object>> property)
        {
            return GetChild(ReflectionHelper.GetProperty(property));
        }

        public Notification Flatten()
        {
            var messages = new List<NotificationMessage>();
            gather(messages);

            var notification = new Notification();
            notification._messages.AddRange(messages);

            return notification;
        }

        private void gather(List<NotificationMessage> messages)
        {
            messages.AddRange(_messages);
            foreach (var pair in _children)
            {
                pair.Value.gather(messages);
            }
        }

        public MessageBag MessagesFor(PropertyInfo property)
        {
            return _bags[property];
        }

        public MessageBag MessagesFor<TARGET>(Expression<Func<TARGET, object>> property)
        {
            return MessagesFor(ReflectionHelper.GetProperty(property));
        }

        public void ForEachProperty(Action<MessageBag> action)
        {
            _bags.Each(action);
        }

        public void AddChild(PropertyInfo property, Notification notification)
        {
            _children.Fill(property, notification);
        }

        public void AddChild<TARGET>(Expression<Func<TARGET, object>> property, Notification notification)
        {
            AddChild(ReflectionHelper.GetProperty(property), notification);
        }

        public bool IsValid()
        {
            if(_children.Any(child => !child.Value.IsValid()))
            {
                return false;
            }

            return _messages.Count == 0;
        }

        public static Notification Valid()
        {
            return new Notification();
        }
    }
}