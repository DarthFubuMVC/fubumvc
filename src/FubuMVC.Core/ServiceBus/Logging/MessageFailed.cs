using System;
using FubuCore;
using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.Services.Messaging.Tracking;

namespace FubuMVC.Core.ServiceBus.Logging
{
    public class MessageFailed : MessageLogRecord
    {
        public EnvelopeToken Envelope { get; set; }
        public Exception Exception { get; set; }

        protected bool Equals(MessageFailed other)
        {
            return Equals(Envelope, other.Envelope) && Equals(Exception, other.Exception);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MessageFailed) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Envelope != null ? Envelope.GetHashCode() : 0)*397) ^ (Exception != null ? Exception.GetHashCode() : 0);
            }
        }

        public override MessageRecord ToRecord()
        {
            return new MessageRecord(Envelope)
            {
                Message = "Message failed!",
                ExceptionText = Exception.ToString()
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
            return string.Format("Message failed: {0}", Envelope);
        }
    }
}