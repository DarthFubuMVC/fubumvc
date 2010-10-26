using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuValidation
{
    public class MessageBag
    {
        private readonly Accessor _accessor;
        private readonly List<NotificationMessage> _messages = new List<NotificationMessage>();

        public MessageBag(Accessor accessor)
        {
            _accessor = accessor;
        }

        public Accessor Accessor
        {
            get { return _accessor; }
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