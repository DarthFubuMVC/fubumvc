using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Logging;

namespace FubuMVC.Core.ServiceBus.Runtime.Delayed
{
    public class DelayedEnvelopeAddedBackToQueue : MessageLogRecord
    {
        public EnvelopeToken Envelope { get; set; }

        protected bool Equals(DelayedEnvelopeAddedBackToQueue other)
        {
            return Equals(Envelope, other.Envelope);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DelayedEnvelopeAddedBackToQueue) obj);
        }

        public override int GetHashCode()
        {
            return (Envelope != null ? Envelope.GetHashCode() : 0);
        }

        public override MessageRecord ToRecord()
        {
            return new MessageRecord(Envelope)
            {
                Message = "Envelope added back to the queue"
            };
        }
    }
}