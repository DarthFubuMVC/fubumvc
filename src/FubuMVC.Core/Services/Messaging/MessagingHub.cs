using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FubuCore;
using Newtonsoft.Json;

namespace FubuMVC.Core.Services.Messaging
{
    public class MessagingHub : IMessagingHub
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly IList<object> _listeners = new List<object>();
        private readonly JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public IEnumerable<object> Listeners
        {
            get
            {
                return _lock.Read(() => {
                    return _listeners.ToArray();
                });
                
            }
        }

        public void AddListener(object listener)
        {
            _lock.Write(() => {
                _listeners.Fill(listener);
            });
            
        }

        public void RemoveListener(object listener)
        {
            _lock.Write(() => {
                _listeners.Remove(listener);
            });
            
        }

        public void Send<T>(T message)
        {
            _listeners.OfType<IListener<T>>().ToArray().Each(x => x.Receive(message));
            _listeners.OfType<IListener>().ToArray().Each(x => x.Receive(message));
        }

        public void SendJson(string json)
        {
            var o = _jsonSerializer.Deserialize(new JsonTextReader(new StringReader(json)));
            typeof(Sender<>).CloseAndBuildAs<ISender>(o.GetType())
                            .Send(o, this);
        }

        public interface ISender
        {
            void Send(object o, MessagingHub listeners);
        }

        public class Sender<T> : ISender
        {
            public void Send(object o, MessagingHub listeners)
            {
                listeners.Send(o.As<T>());
            }
        }
    }
}