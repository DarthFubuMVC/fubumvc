using System;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.ServerSentEvents
{
    public class EventQueueFactory : IEventQueueFactory
    {
        private readonly Cache<Type, object> _factories;

        public EventQueueFactory(IServiceLocator serviceLocator)
        {
            _factories = new Cache<Type, object>(type => serviceLocator.GetInstance(typeof (IEventQueueFactory<>).MakeGenericType(type)));
        }

        public IEventQueue<T> BuildFor<T>(T topic) where T : Topic
        {
            var factory = (IEventQueueFactory<T>) _factories[typeof (T)];
            return factory.BuildFor(topic);
        }
    }
}