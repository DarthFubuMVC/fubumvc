using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Runtime.Logging
{
    public class ListenerCollection : IEnumerable<ILogListener>
    {
        private readonly IEnumerable<ILogListener> _listeners;

        public ListenerCollection(IEnumerable<ILogListener> listeners)
        {
            _listeners = listeners;
        }

        private Action<Func<T>> findAction<T>(Func<ILogListener, bool> filter, Action<ILogListener, T> proceed)
        {
            var listeners = _listeners.Where(filter).ToList();
            if (listeners.Any())
            {
                return source =>
                {
                    var msg = source();
                    listeners.Each(x => proceed(x, msg));
                };
            }

            return msg => { };
        }

        public Action<Func<string>> Debug()
        {
            return findAction<string>(x => x.IsDebugEnabled, (listener, msg) => listener.Debug(msg));
        }

        public Action<Func<string>> Info()
        {
            return findAction<string>(x => x.IsInfoEnabled, (listener, msg) => listener.Info(msg));
        }

        public Action<Func<object>> DebugFor(Type type)
        {
            return findAction<object>(x => x.IsDebugEnabled && x.ListensFor(type), (l, o) => l.DebugMessage(o));
        }

        public Action<Func<object>> InfoFor(Type type)
        {
            return findAction<object>(x => x.IsInfoEnabled && x.ListensFor(type), (l, o) => l.InfoMessage(o));
        }

        public IEnumerator<ILogListener> GetEnumerator()
        {
            return _listeners.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}