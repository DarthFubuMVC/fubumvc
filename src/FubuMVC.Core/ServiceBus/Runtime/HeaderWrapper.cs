﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using FubuCore;
using FubuMVC.Core.ServiceBus.Runtime.Headers;

namespace FubuMVC.Core.ServiceBus.Runtime
{
    [Serializable]
    public class HeaderWrapper
    {
        public string MessageType
        {
            get { return Headers[Envelope.MessageTypeKey]; }
            set
            {
                Headers[Envelope.MessageTypeKey] = value;
            }
        }

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

        public IEnumerable<string> AcceptedContentTypes
        {
            get { return Headers[Envelope.AcceptedContentTypesKey]?.Split(',') ?? Enumerable.Empty<string>(); }
            set { Headers[Envelope.AcceptedContentTypesKey] = value?.Join(","); }
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

    internal static class DictionaryExtensions
    {
        public static Dictionary<string, string> Clone(this IDictionary<string, string> dict)
        {
            return new Dictionary<string, string>(dict);
        }

        public static string Get(this IDictionary<string, string> dict, string key)
        {
            return dict.ContainsKey(key) ? dict[key] : null;
        }

        public static void Set(this IDictionary<string, string> dict, string key, object value)
        {
            if (dict.ContainsKey(key))
            {
                if (value == null)
                {
                    dict.Remove(key);
                }
                else
                {
                    dict[key] = value.ToString();
                }
            }
            else
            {
                dict.Add(key, value?.ToString());
            }
        }

        public static Uri GetUri(this IDictionary<string, string> dict, string key)
        {
            return dict.ContainsKey(key) ? dict[key].ToUri() : null;
        }

        public static int GetInt(this IDictionary<string, string> dict, string key)
        {
            return dict.ContainsKey(key) ? int.Parse(dict[key]) : 0;
        }
    }
}
