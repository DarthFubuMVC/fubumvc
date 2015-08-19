using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FubuCore;

namespace FubuMVC.Core.ServerSentEvents
{
    public class Channel<TTopic> : IChannel<TTopic> where TTopic : Topic
    {
        private readonly IEventQueue<TTopic> _queue;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        
        private readonly IList<QueuedRequest> _outstandingRequests = new List<QueuedRequest>();
        private bool _isConnected;

        public Channel(IEventQueue<TTopic> queue)
        {
            _queue = queue;
            _isConnected = true;
        }

        public void Flush()
        {
            _lock.Write(() =>
            {
                _outstandingRequests.Each(x => x.Source.SetResult(Enumerable.Empty<IServerEvent>()));
                _outstandingRequests.Clear();

                _isConnected = false;
            });
        }

        public Task<IEnumerable<IServerEvent>> FindEvents(TTopic topic)
        {
            var source = new TaskCompletionSource<IEnumerable<IServerEvent>>();

            if (!FindEventsReadLockOnly(topic, source))
                FindEventsWithWriteLock(topic, source);

            return source.Task;
        }

        public void Write(Action<IEventQueue<TTopic>> action)
        {
            _lock.Write(() =>
            {
                action(_queue);
                publishToListeners();
            });
        }

        public bool IsConnected()
        {
            return _isConnected;
        }

        private bool FindEventsReadLockOnly(TTopic topic, TaskCompletionSource<IEnumerable<IServerEvent>> source)
        {
            return _lock.Read(() =>
            {
                if (!_isConnected)
                {
                    source.SetResult(Enumerable.Empty<IServerEvent>());
                    return true;
                }

                var events = _queue.FindQueuedEvents(topic);

                if (!events.Any())
                    return false;

                source.SetResult(events);
                return true;
            });
        }

        private void FindEventsWithWriteLock(TTopic topic, TaskCompletionSource<IEnumerable<IServerEvent>> source)
        {
            _lock.Write(() =>
            {
                if (!FindEventsReadLockOnly(topic, source))
                {
                    _outstandingRequests.Add(new QueuedRequest(source, topic));
                }
            });
        }

        private void publishToListeners()
        {
            _outstandingRequests.Each(x =>
            {
                var events = _queue.FindQueuedEvents(x.Topic);
                x.Source.SetResult(events);
            });

            _outstandingRequests.Clear();
        }

        public class QueuedRequest
        {
            public QueuedRequest(TaskCompletionSource<IEnumerable<IServerEvent>> source, TTopic topic)
            {
                Source = source;
                Topic = topic;
            }

            public TaskCompletionSource<IEnumerable<IServerEvent>> Source { get; private set; }
            public TTopic Topic { get; private set; }
        }


    }
}