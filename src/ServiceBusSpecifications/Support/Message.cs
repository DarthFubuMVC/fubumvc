using System;
using FubuCore;
using FubuMVC.Core.ServiceBus.Runtime;

namespace ServiceBusSpecifications.Support
{
    public class Message
    {
        public Message()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public Uri Source { get; set; }
        public string Key { get; set; }

        public Envelope Envelope { get; set; }

        protected bool Equals(Message other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Message) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            if (Source == null || Key == null)
            {
                return "{0} ({1})".ToFormat(GetType().Name, Id);
            }

            return string.Format("{0} ({1}) from {2}/{3}", GetType().Name, Id, Source, Key);
        }
    }
}