using System.Collections.Generic;

namespace FubuMVC.Core.ServerSentEvents
{
    public class EventQueue<T> : IEventQueue<T> where T : Topic
    {
        private readonly ServerEventList<IServerEvent> _events = new ServerEventList<IServerEvent>();

        public void Clear()
        {
            _events.AllEvents.Clear();
        }

        public IList<IServerEvent> AllEvents
        {
            get { return _events.AllEvents; }
        }

        public IEnumerable<IServerEvent> FindQueuedEvents(T topic)
        {
            return _events.FindQueuedEvents(topic);
        }

        public void Write(params IServerEvent[] events)
        {
            _events.Add(events);
        }
    }
}