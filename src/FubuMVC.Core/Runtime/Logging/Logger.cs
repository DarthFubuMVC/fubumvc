using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Runtime.Logging
{
    public class Logger : ILogger
    {
        private readonly ListenerCollection _listeners;
        private readonly Lazy<Action<Func<string>>> _debugString;
        private readonly Lazy<Action<Func<string>>> _infoString;
        private readonly Cache<Type, Action<Func<object>>> _debugMessage = new Cache<Type,Action<Func<object>>>();
        private readonly Cache<Type, Action<Func<object>>> _infoMessage = new Cache<Type,Action<Func<object>>>();

        public Logger(IEnumerable<ILogListener> listeners)
        {
            _listeners = new ListenerCollection(listeners);
            _debugString = new Lazy<Action<Func<string>>>(() => _listeners.Debug());
            _infoString = new Lazy<Action<Func<string>>>(() => _listeners.Info());

            _debugMessage.OnMissing = type => _listeners.DebugFor(type);
            _infoMessage.OnMissing = type => _listeners.InfoFor(type);
        }

        public void Debug(string message, params object[] parameters)
        {
            Debug(() => message.ToFormat(parameters));
        }

        public void Info(string message, params object[] parameters)
        {
            Info(() => message.ToFormat(parameters));
        }

        public void Error(string message, Exception ex)
        {
            _listeners.Each(x => x.Error(message, ex));
        }

        public void Error(object correlationId, string message, Exception ex)
        {
            _listeners.Each(x => x.Error(correlationId, message, ex));
        }

        public void Debug(Func<string> message)
        {
            _debugString.Value(message);
        }

        public void Info(Func<string> message)
        {
            _infoString.Value(message);
        }

        public void DebugMessage(object message)
        {
            if (message == null)
            {
                return;
            }

            _debugMessage[message.GetType()](() => message);
        }

        public void InfoMessage(object message)
        {
            if (message == null)
            {
                return;
            }

            _infoMessage[message.GetType()](() => message);
        }

        public void DebugMessage<T>(Func<T> message)
        {
            _debugMessage[typeof (T)](() => message());
        }

        public void InfoMessage<T>(Func<T> message)
        {
            _infoMessage[typeof (T)](() => message());
        }
    }
}