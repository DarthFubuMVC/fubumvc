using System;
using FubuCore;

namespace FubuMVC.Core.ServerSentEvents
{
    public class EventPublisher : IEventPublisher
    {
        private readonly ITopicChannelCache _cache;

        public EventPublisher(ITopicChannelCache cache)
        {
            _cache = cache;
        }

        public void WriteTo<T>(T topic, params IServerEvent[] events) where T : Topic
        {
            _cache.ChannelFor(topic).WriteEvents(topic, events);
        }

        public void WriteTo<T, TQueue>(T topic, Action<TQueue> write) where T : Topic where TQueue : class
        {
            var channel = _cache.ChannelFor(topic).Channel;
            channel.Write(q => (q as TQueue).CallIfNotNull(write));
        }
    }
}