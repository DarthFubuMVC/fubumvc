using System;
using System.Reflection;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Events;

namespace FubuMVC.Core.ServiceBus.Logging
{
    public class EventAggregationListener : ILogListener
    {
        private readonly IEventAggregator _events;
        private readonly TransportSettings _settings;

        public EventAggregationListener(IEventAggregator events, TransportSettings settings)
        {
            _events = events;
            _settings = settings;
        }

        public bool ListensFor(Type type)
        {
            return type.Assembly.GetName().Name == Assembly.GetExecutingAssembly().GetName().Name;
        }

        public void DebugMessage(object message)
        {
            _events.RouteMessage(message);
        }

        public void InfoMessage(object message)
        {
            _events.RouteMessage(message);
        }

        public void Debug(string message)
        {
            // no-op
        }

        public void Info(string message)
        {
            // no-op
        }

        public void Error(string message, Exception ex)
        {

        }

        public void Error(object correlationId, string message, Exception ex)
        {

        }

        public bool IsDebugEnabled { get { return _settings.DebugEnabled; } }
        public bool IsInfoEnabled { get { return true; } }
    }
}