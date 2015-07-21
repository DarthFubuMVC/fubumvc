using System;
using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Core.ServiceBus.Events
{
    public interface IEventAggregator
    {
        // Sending messages
        void SendMessage<T>(T message);
        void SendMessage<T>() where T : new();

        // Explicit registration
        void AddListener(object listener);
        void RemoveListener(object listener);

        IEnumerable<object> Listeners { get; }

        void PruneExpiredListeners(DateTime currentTime);
    }


    public static class EventAggregatorExtensions
    {
        public static void RouteMessage(this IEventAggregator events, object message)
        {
            typeof(Sender<>).CloseAndBuildAs<ISender>(events, message.GetType())
                .Send(message);
        }

        internal interface ISender
        {
            void Send(object o);
        }

        internal class Sender<T> : ISender
        {
            private readonly IEventAggregator _events;

            public Sender(IEventAggregator events)
            {
                _events = events;
            }

            public void Send(object o)
            {
                _events.SendMessage((T) o);
            }
        }
    }

    

    
}