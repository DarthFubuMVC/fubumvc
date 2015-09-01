using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Http;
using FubuMVC.Core.ServiceBus.Runtime.Headers;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;

namespace FubuMVC.Core.ServiceBus.Runtime
{
    public class Envelope : HeaderWrapper
    {
        public static readonly string OriginalIdKey = "original-id";
        public static readonly string IdKey = "id";
        public static readonly string ParentIdKey = "parent-id";
        public static readonly string ContentTypeKey = HttpResponseHeaders.ContentType;
        public static readonly string SourceKey = "source";
        public static readonly string ChannelKey = "channel";
        public static readonly string ReplyRequestedKey = "reply-requested";
        public static readonly string ResponseIdKey = "response";
        public static readonly string DestinationKey = "destination";
        public static readonly string ReplyUriKey = "reply-uri";
        public static readonly string ExecutionTimeKey = "time-to-send";
        public static readonly string ReceivedAtKey = "received-at";
        public static readonly string AttemptsKey = "attempts";
        public static readonly string AckRequestedKey = "ack-requested";
        public static readonly string MessageTypeKey = "message-type";

        public byte[] Data;

        [NonSerialized]
        private Lazy<object> _message;

        /// <summary>
        /// Used internally for logging
        /// </summary>
        [NonSerialized] internal ChainExecutionLog Log;
            
        public object Message
        {
            get { return _message == null ? null : _message.Value; }   
            set
            {
                if (value == null)
                {
                    _message = null;
                }
                else
                {
                    _message = new Lazy<object>(() => value);
                }
            }
        }

        public void UseSerializer(IEnvelopeSerializer serializer)
        {
            _message = new Lazy<object>(() => serializer.Deserialize(this));
        }


        [NonSerialized] private IMessageCallback _callback;
        public int Attempts
        {
            get { return Headers.Has(AttemptsKey) ? int.Parse(Headers[AttemptsKey]) : 0; }
            set { Headers[AttemptsKey] = value.ToString(); }
        }

        // TODO -- do routing slip tracking later

        public Envelope(IHeaders headers)
        {
            Headers = headers;

            if (CorrelationId.IsEmpty())
            {
                CorrelationId = Guid.NewGuid().ToString();
            }
            
        }

        public Envelope() : this(new NameValueHeaders())
        {

        }

        public Envelope(byte[] data, IHeaders headers, IMessageCallback callback) : this(headers)
        {
            Data = data;
            Callback = callback;
        }

        public IMessageCallback Callback
        {
            get { return _callback; }
            set { _callback = value; }
        }

        public bool MatchesResponse(object message)
        {
            return message.GetType().Name == ReplyRequested;
        }

        // TODO -- this is where the routing slip is going to come into place
        
        public virtual Envelope ForResponse(object message)
        {
            var child = ForSend(message);

            if (MatchesResponse(message))
            {
                child.Headers[ResponseIdKey] = CorrelationId;
                child.Destination = ReplyUri;
            }

            return child;
        }

        public virtual Envelope ForSend(object message)
        {
            var child = new Envelope
            {
                Message = message,
                OriginalId = OriginalId ?? CorrelationId,
                ParentId = CorrelationId
            };
            return child;
        }

        public override string ToString()
        {
            var id = ResponseId.IsNotEmpty()
                ? "{0} in response to {1}".ToFormat(CorrelationId, ResponseId) : CorrelationId;

            if (Message != null)
            {
                return string.Format("Envelope for message {0} ({1}) w/ Id {2}", Message, Message.GetType().Name, id);
            }
            else
            {
                return "Envelope w/ Id {0}".ToFormat(id);
            }
        }

        public Envelope Clone()
        {
            var clone = new Envelope
            {
                Message = Message
            };

            Headers.Keys().Each(key => clone.Headers[key] = Headers[key]);

            return clone;
        }

        public EnvelopeToken ToToken()
        {
            return new EnvelopeToken
            {
                Data = Data,
                Headers = Headers,
                MessageSource = _message
            };

            
        }

        protected bool Equals(Envelope other)
        {
            return Equals(Data, other.Data) && Equals(Message, other.Message) && Equals(_callback, other._callback) && Equals(Headers, other.Headers);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Envelope) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Data != null ? Data.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_message != null ? _message.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_callback != null ? _callback.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

}