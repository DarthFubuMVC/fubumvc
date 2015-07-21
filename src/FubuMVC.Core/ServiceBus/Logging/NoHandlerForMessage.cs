using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus.Logging
{
    public class NoHandlerForMessage : MessageLogRecord
    {
        public EnvelopeToken Envelope { get; set; }

        protected bool Equals(NoHandlerForMessage other)
        {
            return Equals(Envelope, other.Envelope);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NoHandlerForMessage) obj);
        }

        public override int GetHashCode()
        {
            return (Envelope != null ? Envelope.GetHashCode() : 0);
        }

        public override MessageRecord ToRecord()
        {
            return new MessageRecord(Envelope)
            {
                Message = "No Handler for Message"
            };
        }

        public override string ToString()
        {
            return string.Format("No handler for message: {0}", Envelope);
        }
    }
}