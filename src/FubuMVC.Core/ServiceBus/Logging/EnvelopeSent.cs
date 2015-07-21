using System;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus.Logging
{
    public class EnvelopeSent : MessageLogRecord
    {
        public EnvelopeSent()
        {
        }

        public EnvelopeSent(EnvelopeToken envelope, ChannelNode node)
        {
            Envelope = envelope;

            Uri = node.Uri;
            Key = node.Key;
        }

        public EnvelopeToken Envelope { get; set; }
        public Uri Uri { get; set; }
        public string Key { get; set; }

        protected bool Equals(EnvelopeSent other)
        {
            return Equals(Envelope, other.Envelope) && Equals(Uri, other.Uri) && string.Equals(Key, other.Key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EnvelopeSent) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Envelope != null ? Envelope.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Uri != null ? Uri.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Key != null ? Key.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override MessageRecord ToRecord()
        {
            return new MessageRecord(Envelope)
            {
                Message = "Envelope Sent"
            };
        }

        public override string ToString()
        {
            return string.Format("Sent {0} to {1} ({2})", Envelope, Uri, Key);
        }
    }
}