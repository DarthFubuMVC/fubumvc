using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation.Batching
{
    public abstract class BatchMessage : IBatchMessage
    {
        private readonly IList<object> _messages = new List<object>();

        public BatchMessage()
        {
        }

        public BatchMessage(params object[] messages)
        {
            _messages.AddRange(messages);
        }

        public object[] Messages
        {
            get { return _messages.ToArray(); }
            set
            {
                _messages.Clear();
                _messages.AddRange(value);
            }
        }

        public void Add(object message)
        {
            if (message == null) throw new ArgumentNullException("message");

            _messages.Add(message);
        }
    }
}