using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FubuMVC.Core.Services;
using FubuMVC.Core.Services.Messaging;

namespace RemoteService
{
    public class RemoteGo
    {
        public Guid Id = Guid.NewGuid();

        protected bool Equals(RemoteGo other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RemoteGo) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Id: {0}", Id);
        }
    }

    public class RemoteApplication : IApplicationLoader, IDisposable, IListener<RemoteGo>
    {
        public IDisposable Load(Dictionary<string, string> properties)
        {
            EventAggregator.Messaging.AddListener(this);
            return this;
        }

        public void Dispose()
        {
            
        }

        public void Receive(RemoteGo message)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                EventAggregator.ReceivedMessage(message);
            });
        }
    }
}