using System.Collections.Generic;
using System.Reflection;

namespace FubuValidation
{
    public class MessageBag
    {
        private readonly PropertyInfo _property;
        private readonly List<NotificationMessage> _messages = new List<NotificationMessage>();

        public MessageBag(PropertyInfo property)
        {
            _property = property;
        }

        public PropertyInfo Property
        {
            get { return _property; }
        }

        public NotificationMessage[] Messages
        {
            get { return _messages.ToArray(); }
        }

        public void Add(NotificationMessage message)
        {
            _messages.Add(message);
        }

        public bool Contains(NotificationMessage message)
        {
            return _messages.Contains(message);
        }
    }
}