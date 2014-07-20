using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using StructureMap.Util;

namespace FubuMVC.Core.Diagnostics.Runtime
{
    public class RequestLog
    {
        private readonly IList<RequestStep> _steps = new List<RequestStep>();

        public RequestLog()
        {
            Id = Guid.NewGuid();
            ResponseHeaders = Enumerable.Empty<Header>();

            StatusCode = 200;
            StatusDescription = "OK";
        }

        public Guid Id { get; private set; }
        public int Hash { get; set; }

        public double ExecutionTime { get; set; }
        public string Querystring { get; set; }

        public string Endpoint { get; set; }
        public string HttpMethod { get; set; }
        public DateTime Time { get; set; }

        public IDictionary<string, object> ToDictionary()
        {
            var endpoint = Endpoint;
            if (endpoint.IsEmpty()) endpoint = "(home)";

            return new Dictionary<string, object>
            {
                {"time", LocalTime},
                {"endpoint", endpoint},
                {"method", HttpMethod},
                {"status", StatusCode},
                {"description", StatusDescription},
                {"content-type", ContentType},
                {"duration", ExecutionTime},
                {"id", Id.ToString()},
                {"hash", Hash}
            };
        }

        public virtual void AddLog(double requestTimeInMilliseconds, object log)
        {
            _steps.Add(new RequestStep(requestTimeInMilliseconds, log));
        }


        public string LocalTime
        {
            get { return Time.ToLocalTime().ToLongTimeString(); }
        }

        public string ContentType
        {
            get
            {
                if (ResponseHeaders == null)
                {
                    return "Unknown";
                }

                var header = ResponseHeaders
                    .FirstOrDefault(
                        x => x.Name.Equals(HttpResponseHeaders.ContentType, StringComparison.InvariantCultureIgnoreCase));

                return header == null ? "Unknown" : header.Value;
            }
        }

        public bool Failed { get; set; }

        public IEnumerable<Header> ResponseHeaders { get; set; }
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public IEnumerable<Header> RequestHeaders { get; set; }
        public NameValueCollection FormData { get; set; }
        public NameValueCollection QueryString { get; set; }

        public IEnumerable<RequestStep> AllSteps()
        {
            return _steps;
        }

        public IEnumerable<T> AllLogsOfType<T>()
        {
            return _steps.Where(x => x.Log is T).Select(x => x.Log).OfType<T>();
        }

        public bool Equals(RequestLog other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id.Equals(Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RequestLog)) return false;
            return Equals((RequestLog) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Id: {0}", Id);
        }
    }


}