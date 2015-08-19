using System.Collections.Generic;

namespace FubuMVC.Core.ServerSentEvents
{
    public interface IEventQueue<in T> where T : Topic
    {
        IEnumerable<IServerEvent> FindQueuedEvents(T topic);
        void Write(params IServerEvent[] events);
    }
}