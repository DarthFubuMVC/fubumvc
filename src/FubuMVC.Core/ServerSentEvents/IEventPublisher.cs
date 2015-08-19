using System;

namespace FubuMVC.Core.ServerSentEvents
{
    public interface IEventPublisher
    {
        void WriteTo<T>(T topic, params IServerEvent[] events) where T : Topic;

        void WriteTo<T, TQueue>(T topic, Action<TQueue> write)
            where T : Topic
            where TQueue : class;
    }
}