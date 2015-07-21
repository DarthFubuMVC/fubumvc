using System;
using System.Globalization;
using System.Xml;
using FubuCore;
using FubuMVC.Core.ServiceBus.Runtime.Headers;

namespace FubuMVC.Core.ServiceBus.Runtime
{
    [Serializable]
    public class HeaderWrapper
    {
        public Uri Source
        {
            get { return Headers[Envelope.SourceKey].ToUri(); }
            set { Headers[Envelope.SourceKey] = value == null ? null : value.ToString(); }
        }

        public Uri ReplyUri
        {
            get { return Headers[Envelope.ReplyUriKey].ToUri(); }
            set { Headers[Envelope.ReplyUriKey] = value == null ? null : value.ToString(); }
        }

        public string ContentType
        {
            get { return Headers[Envelope.ContentTypeKey]; }
            set { Headers[Envelope.ContentTypeKey] = value; }
        }

        public string OriginalId
        {
            get { return Headers[Envelope.OriginalIdKey]; }
            set { Headers[Envelope.OriginalIdKey] = value; }
        }

        public string ParentId
        {
            get { return Headers[Envelope.ParentIdKey]; }
            set { Headers[Envelope.ParentIdKey] = value; }
        }

        public string ResponseId
        {
            get { return Headers[Envelope.ResponseIdKey]; }
            set { Headers[Envelope.ResponseIdKey] = value; }
        }

        public Uri Destination
        {
            get { return Headers[Envelope.DestinationKey].ToUri(); }
            set { Headers[Envelope.DestinationKey] = value == null ? null : value.ToString(); }
        }

        public Uri ReceivedAt
        {
            get { return Headers[Envelope.ReceivedAtKey].ToUri(); }
            set { Headers[Envelope.ReceivedAtKey] = value == null ? null : value.ToString(); }
        }

        public IHeaders Headers { get; set; }

        public string CorrelationId
        {
            get
            {
                return Headers[Envelope.IdKey];
            }
            set { Headers[Envelope.IdKey] = value; }
        }

        public string ReplyRequested
        {
            get
            {
                return Headers[Envelope.ReplyRequestedKey];
            }
            set { Headers[Envelope.ReplyRequestedKey] = value; }
        }

        public bool AckRequested
        {
            get { return Headers.Has(Envelope.AckRequestedKey) ? Headers[Envelope.AckRequestedKey].EqualsIgnoreCase("true") : false; }
            set
            {
                if (value)
                {
                    Headers[Envelope.AckRequestedKey] = "true";
                }
                else
                {
                    Headers.Remove(Envelope.AckRequestedKey);
                }
            }
        }

        public DateTime? ExecutionTime
        {
            get { return Headers.Has(Envelope.ExecutionTimeKey) ? XmlConvert.ToDateTime(Headers[Envelope.ExecutionTimeKey], XmlDateTimeSerializationMode.Utc) : (DateTime?)null; }
            set
            {
                if (value == null)
                {
                    Headers.Remove(Envelope.ExecutionTimeKey);
                }
                else
                {
                    Headers[Envelope.ExecutionTimeKey] = value.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture);
                }
                
            }
        }

        public bool IsDelayed(DateTime utcNow)
        {
            if (!Headers.Has(Envelope.ExecutionTimeKey)) return false;

            return ExecutionTime.Value > utcNow;
        }
    }
}