using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Events;

namespace FubuMVC.Core.ServiceBus
{

    public class EventAggregator : IEventAggregator
    {
        private readonly Lazy<ILogger> _logger;
        private readonly List<object> _listeners = new List<object>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public EventAggregator(Func<ILogger> logger, IEnumerable<IListener> listeners)
        {
            _logger = new Lazy<ILogger>(logger);
            _listeners.AddRange(listeners);
        }

        public virtual void SendMessage<T>(T message)
        {
            Task.Factory.StartNew(() => sendMessageToListeners(message));
        }

        protected void sendMessageToListeners<T>(T message)
        {
            var listeners = _lock.Read(() => _listeners.OfType<IListener<T>>().ToArray());

            listeners.Each(x => {
                try
                {
                    x.Handle(message);
                }
                catch (Exception e)
                {
                    _logger.Value.Error("Failed while trying to process event {0} with listener {1}".ToFormat(message, x), e);
                }
            });
        }

        public void SendMessage<T>() where T : new()
        {
            SendMessage(new T());
        }

        public void AddListener(object listener)
        {
            _lock.Write(() => _listeners.Fill(listener));
        }

        public void RemoveListener(object listener)
        {
            _lock.Write(() => _listeners.Remove(listener));
        }

        public IEnumerable<object> Listeners
        {
            get { return _lock.Read(() => _listeners.ToArray()); }
        }

        public void PruneExpiredListeners(DateTime currentTime)
        {
            _lock.Write(() => _listeners.RemoveAll(o => o.IsExpired(currentTime)));
        }

        public void AddListeners(params object[] listeners)
        {
            _lock.Write(() => _listeners.Fill(listeners));
        }

        public bool HasListener(object listener)
        {
            return _lock.Read(() => _listeners.Contains(listener));
        }

        public void RemoveAllListeners()
        {
            _lock.Write(() => _listeners.Clear());
        }
    }
}