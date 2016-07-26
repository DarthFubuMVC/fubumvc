using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Events;

namespace FubuMVC.Core.ServiceBus
{
    public class EventAggregator : IEventAggregator
    {
        private readonly Lazy<ILogger> _logger;
        private readonly ConcurrentDictionary<object, object> _listeners = new ConcurrentDictionary<object, object>();

        public EventAggregator(Func<ILogger> logger, IEnumerable<IListener> listeners)
        {
            _logger = new Lazy<ILogger>(logger);
            foreach (var listener in listeners)
            {
                addListener(listener);
            }
        }

        private void addListener(object listener)
        {
            _listeners.AddOrUpdate(listener, listener, (a, b) => listener);
        }

        public virtual void SendMessage<T>(T message)
        {
            Task.Factory.StartNew(() => sendMessageToListeners(message));
        }

        protected void sendMessageToListeners<T>(T message)
        {
            //Performance critical, keep this way
            foreach (var kvp in _listeners)
            {
                var listener = kvp.Key as IListener<T>;
                if (listener == null)
                {
                    continue;
                }
                try
                {
                    listener.Handle(message);
                }
                catch (Exception e)
                {
                    _logger.Value.Error(
                        "Failed while trying to process event {0} with listener {1}".ToFormat(message, listener), e);
                }
            }
        }

        public void SendMessage<T>() where T : new()
        {
            SendMessage(new T());
        }

        public void AddListener(object listener)
        {
            _listeners.AddOrUpdate(listener, listener, (a, b) => b);
        }

        public void RemoveListener(object listener)
        {
            _listeners.TryRemove(listener, out listener);
        }

        public IEnumerable<object> Listeners
        {
            get { return _listeners.Select(x => x.Key).ToArray(); }
        }

        public void PruneExpiredListeners(DateTime currentTime)
        {
            var items = new List<object>();
            foreach (var listener in _listeners)
            {
                var key = listener.Key;
                if (key.IsExpired(currentTime))
                {
                    items.Add(key);
                }
            }
            foreach (var listener in items)
            {
                RemoveListener(listener);
            }
        }

        public void AddListeners(params object[] listeners)
        {
            foreach (var listener in listeners)
            {
                addListener(listener);
            }
        }

        public bool HasListener(object listener)
        {
            return _listeners.ContainsKey(listener);
        }

        public void RemoveAllListeners()
        {
            _listeners.Clear();
        }
    }
}