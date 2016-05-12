using System;
using FubuCore;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.Services.Messaging.Tracking;

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

        public override MessageTrack ToMessageTrack()
        {
            return  new MessageTrack
            {
                Type = "OutstandingEnvelope",
                Id = Envelope.CorrelationId,
                FullName = "{0}@{1}".ToFormat(Envelope.CorrelationId, Uri),
                Status = MessageTrack.Sent
            };
        }

        public override string ToString()
        {
            return $"Sent {Envelope} to {Uri} ({Key})";
        }
    }
}