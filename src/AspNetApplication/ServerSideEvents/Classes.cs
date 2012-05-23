using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FubuMVC.Core.Http;

using Events = System.Collections.Generic.IEnumerable<AspNetApplication.ServerSideEvents.ServerEvent>;
using FubuCore;

namespace AspNetApplication.ServerSideEvents
{
    public interface IEventSource<in T> where T : Topic
    {
        Task<IEnumerable<ServerEvent>> FindEvents(T topic);        
    }

    public class EventSource<T> : IEventSource<T> where T : Topic
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly IList<ServerEvent> _events = new List<ServerEvent>();
        private readonly IList<QueuedRequest> _outstandingRequests = new List<QueuedRequest>();

        public Task<Events> FindEvents(T topic)
        {
            var events = _lock.Read(() => FindQueuedEvents(topic));

            var source = new TaskCompletionSource<Events>();
            if (events.Any())
            {
                source.SetResult(events);
            }
            else
            {
                _lock.Write(() => _outstandingRequests.Add(new QueuedRequest(source, topic)));
            }

            return source.Task;
        }

        public void ClearAll()
        {
            _lock.Write(() => _outstandingRequests.Clear());
        }



        public void QueueEvents(Events events)
        {
            modifyEvents(list => list.AddRange(events));
        }

        public void QueueEvent(ServerEvent @event)
        {
            modifyEvents(list => list.Add(@event));
        }

        protected void modifyEvents(Action<IList<ServerEvent>> modification)
        {
            _lock.Write(() =>
            {
                modification(_events);
                publishToListeners();
            });
        }

        private void publishToListeners()
        {
            _outstandingRequests.Each(x =>
            {
                var events = FindQueuedEvents(x.Topic);
                x.Source.SetResult(events);
            });

            _outstandingRequests.Clear();
        }

        public Events FindQueuedEvents(T topic)
        {
            if (topic.LastEventId.IsEmpty())
            {
                return _events.ToList();
            }

            if (!_events.Any() || _events.Last().Id == topic.LastEventId) return Enumerable.Empty<ServerEvent>();

            var lastEvent = _events.FirstOrDefault(x => x.Id == topic.LastEventId);
            if (lastEvent == null)
            {
                return _events.ToList();
            }

            var index = _events.IndexOf(lastEvent);
            return _events.Skip(index + 1).ToList();
        }


        public class QueuedRequest
        {
            public QueuedRequest(TaskCompletionSource<Events> source, T topic)
            {
                Source = source;
                Topic = topic;
            }

            public TaskCompletionSource<Events> Source { get; private set; }
            public T Topic { get; private set; }
        }
    }

    public class EventSourceSettings
    {
        public int MaximumMessages { get; set; }
        
    }



    public class EventSourceWriter<T> where T : Topic
    {
        private readonly IClientConnectivity _connectivity;
        private readonly IServerEventWriter _writer;
        private readonly IEventSource<T> _source;

        public EventSourceWriter(IClientConnectivity connectivity, IServerEventWriter writer, IEventSource<T> source)
        {
            _connectivity = connectivity;
            _writer = writer;
            _source = source;
        }

        public void WriteMessages(T topic)
        {
            while (_connectivity.IsClientConnected())
            {
                var task = _source.FindEvents(topic);

                // Needs to deal w/ timeouts
                while (!task.Wait(1000))
                {
                    if (!_connectivity.IsClientConnected()) return;
                }

                var messages = task.Result;
                messages.Each(x => _writer.Write(x));

                if (messages.Any())
                {
                    topic.LastEventId = messages.Last().Id;
                }
            }
        }

        public Task Write(T topic)
        {
            return Task.Factory.StartNew(() => WriteMessages(topic));
        }
    }
}