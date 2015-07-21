using System;
using FubuCore;
using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus.Logging
{
    public class EnvelopeContinuationChosen : MessageLogRecord
    {
        public EnvelopeToken Envelope;
        public Type HandlerType;
        public Type ContinuationType;

        public override MessageRecord ToRecord()
        {
            return new MessageRecord(Envelope)
            {
                Message = "Chose continuation {0} from handler {1}".ToFormat(ContinuationType.Name, HandlerType.Name)
            };
        }

        protected bool Equals(EnvelopeContinuationChosen other)
        {
            return Equals(Envelope, other.Envelope) && Equals(HandlerType, other.HandlerType) && Equals(ContinuationType, other.ContinuationType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EnvelopeContinuationChosen) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Envelope != null ? Envelope.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (HandlerType != null ? HandlerType.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ContinuationType != null ? ContinuationType.GetHashCode() : 0);
                return hashCode;
            }
        }



        public override string ToString()
        {
            return "Chose continuation {0} for envelope {1}".ToFormat(ContinuationType, Envelope);
        }
    }
}