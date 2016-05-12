using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.Services.Messaging.Tracking;

namespace FubuMVC.Core.ServiceBus.Logging
{
    public class EnvelopeReceived : MessageLogRecord
    {
        public EnvelopeToken Envelope { get; set; }

        protected bool Equals(EnvelopeReceived other)
        {
            return Equals(Envelope, other.Envelope);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EnvelopeReceived) obj);
        }

        public override int GetHashCode()
        {
            return (Envelope != null ? Envelope.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Envelope received for {0} from {1} at {2}", Envelope.Message, Envelope.ReplyUri, Envelope.ReceivedAt);
        }

        public override MessageRecord ToRecord()
        {
            return new MessageRecord(Envelope)
            {
                Message = "Envelope Received"
            };
        }

        public override MessageTrack ToMessageTrack()
        {
            return null;
        }
    }
}