using FubuCore;
using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.Services.Messaging.Tracking;

namespace FubuMVC.Core.ServiceBus.Logging
{
    public class MessageSuccessful : MessageLogRecord
    {
        public EnvelopeToken Envelope { get; set; }

        protected bool Equals(MessageSuccessful other)
        {
            return Equals(Envelope, other.Envelope);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MessageSuccessful) obj);
        }

        public override int GetHashCode()
        {
            return (Envelope != null ? Envelope.GetHashCode() : 0);
        }

        public override MessageRecord ToRecord()
        {
            return new MessageRecord(Envelope)
            {
                Message = "Message succeeded"
            };
        }

        public override MessageTrack ToMessageTrack()
        {
            return new MessageTrack
            {
                Type = "OutstandingEnvelope",
                Id = Envelope.CorrelationId,
                FullName = "{0}@{1}".ToFormat(Envelope.CorrelationId, Envelope.Destination),
                Status = MessageTrack.Received
            };
        }

        public override string ToString()
        {
            return $"Message {Envelope} succeeded at {Envelope.ReceivedAt}";
        }


    }
}