using System;
using FubuCore;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using FubuMVC.Core.ServiceBus.Runtime.Headers;

namespace FubuMVC.Core.ServiceBus.Runtime
{
    [Serializable]
    public class EnvelopeToken : HeaderWrapper
    {
        public EnvelopeToken()
        {
            Headers = new NameValueHeaders();
            CorrelationId = Guid.NewGuid().ToString();
        }

        public byte[] Data;
        public Lazy<object> MessageSource
        {
            set
            {
                _message = value;
            }
        }

        [NonSerialized]
        private Lazy<object> _message;

        public object Message
        {
            get { return _message == null ? null : _message.Value; }
            set
            {
                _message = new Lazy<object>(() => value);
            }
        }

        public bool IsPollingJobRelated()
        {
            return Message == null ? false : Message.GetType().Closes(typeof (JobRequest<>));
        }

        public bool IsDelayedEnvelopePollingJobRelated()
        {
            return Message == null ? false : Message.GetType() == typeof(JobRequest<DelayedEnvelopeProcessor>);
        }

        protected bool Equals(EnvelopeToken other)
        {
            return Equals(Data, other.Data) && Equals(Headers, other.Headers);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EnvelopeToken)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Data != null ? Data.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_message != null ? _message.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Headers != null ? Headers.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} from {1}", Message, ReplyUri);
        }
    }
}