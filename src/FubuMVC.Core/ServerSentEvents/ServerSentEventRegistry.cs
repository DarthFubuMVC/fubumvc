using System;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.ServerSentEvents
{
    public class ServerSentEventRegistry : ServiceRegistry
    {
        public ServerSentEventRegistry()
        {
            SetServiceIfNone<IEventPublisher, EventPublisher>();
            SetServiceIfNone<IEventQueueFactory, EventQueueFactory>();
            For(typeof (IEventQueueFactory<>)).Use(typeof (DefaultEventQueueFactory<>));

            For(typeof (IChannelInitializer<>)).Use(typeof (DefaultChannelInitializer<>));

            SetServiceIfNone<IServerEventWriter, ServerEventWriter>();
            SetServiceIfNone<ITopicChannelCache, TopicChannelCache>().Singleton();
            if (Type.GetType("Mono.Runtime") == null)
            {
                SetServiceIfNone<IAspNetShutDownDetector, AspNetShutDownDetector>();
            }
            else
            {
                SetServiceIfNone<IAspNetShutDownDetector, NulloShutdownDetector>();
            }
        }
    }
}