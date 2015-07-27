using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Monitoring;

namespace ServiceBusSpecifications.Fixtures.Monitoring
{
    public class PersistentTaskMessageListener : ILogListener
    {
        private readonly ConcurrentBag<PersistentTaskMessage> _messages = new ConcurrentBag<PersistentTaskMessage>();

        public bool ListensFor(Type type)
        {
            return type.CanBeCastTo<PersistentTaskMessage>();
        }

        public void DebugMessage(object message)
        {
            _messages.Add((PersistentTaskMessage) message);
        }

        public void InfoMessage(object message)
        {
            _messages.Add((PersistentTaskMessage)message);
        }

        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message, Exception ex)
        {
            Console.WriteLine(message);
            Console.WriteLine(ex);
        }

        public void Error(object correlationId, string message, Exception ex)
        {
            Console.WriteLine(correlationId);
            Console.WriteLine(message);

            Console.WriteLine(ex);
        }

        public bool IsDebugEnabled
        {
            get
            {
                return true;
            }
        }

        public bool IsInfoEnabled
        {
            get
            {
                return true;
            }
        }

        public IEnumerable<PersistentTaskMessage> LoggedEvents()
        {
            return _messages.ToArray().OrderBy(x => x.Time);
        }
    }
}