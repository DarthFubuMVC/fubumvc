using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using FubuCore;
using FubuMVC.Core.Http;
using System.Linq;

namespace FubuMVC.OwinHost
{

    public class OwinCurrentHttpRequest : ICurrentHttpRequest
    {
        private readonly IDictionary<string, object> _environment;
        private readonly Lazy<IDictionary<string, string[]>> _headers;
        private readonly Lazy<NameValueCollection> _querystring;

        public OwinCurrentHttpRequest() : this(new Dictionary<string, object>())
        {
            _environment.Add(OwinConstants.RequestHeadersKey, new Dictionary<string, string[]>());
            _querystring = new Lazy<NameValueCollection>(() => new NameValueCollection());
            _environment.Add(OwinConstants.ResponseBodyKey, new MemoryStream());
        }

        public OwinCurrentHttpRequest(IDictionary<string, object> environment)
        {
            _environment = environment;
            _headers = new Lazy<IDictionary<string, string[]>>(() => {
                return environment.Get<IDictionary<string, string[]>>(OwinConstants.RequestHeadersKey);
            });

            _querystring = new Lazy<NameValueCollection>(() => {
                return HttpUtility.ParseQueryString(environment.Get<string>(OwinConstants.RequestQueryStringKey));
            });
        }

        public IDictionary<string, object> Environment
        {
            get { return _environment; }
        }

        private T get<T>(string key)
        {
            object value;
            return _environment.TryGetValue(key, out value) ? (T)value : default(T);
        }

        private void append(string key, object o)
        {
            if (_environment.ContainsKey(key))
            {
                _environment[key] = o;
            }
            else
            {
                _environment.Add(key, o);
            }
        }

        public string RawUrl()
        {
            var queryString = get<string>(OwinConstants.RequestQueryStringKey);
            if (string.IsNullOrEmpty(queryString))
            {
                return get<string>(OwinConstants.RequestPathBaseKey) + get<string>(OwinConstants.RequestPathKey);
            }
            return get<string>("owin.RequestPathBase") + get<string>("owin.RequestPath") + "?" + get<string>("owin.RequestQueryString");
        }

        public string RelativeUrl()
        {
            return _environment.Get<string>(OwinConstants.RequestPathKey).TrimStart('/');
        }

        public string FullUrl()
        {
            var requestPath = get<string>(OwinConstants.RequestPathKey);


            var uriBuilder = uriBuilderFor(requestPath);


            var requestQueryString = get<string>(OwinConstants.RequestQueryStringKey);
            if (!String.IsNullOrEmpty(requestQueryString))
            {
                uriBuilder.Query = requestQueryString;
            }
            return uriBuilder.Uri.ToString();
        }

        private UriBuilder uriBuilderFor(string requestPath)
        {
            var requestScheme = get<string>(OwinConstants.RequestSchemeKey);
            var requestPathBase = get<string>(OwinConstants.RequestPathBaseKey);

            // default values, in absence of a host header
            string host = "127.0.0.1";
            int port = String.Equals(requestScheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ? 443 : 80;

            // if a single host header is available
            string[] hostAndPort;
            if (_headers.Value.TryGetValue("Host", out hostAndPort) &&
                hostAndPort != null &&
                hostAndPort.Length == 1 &&
                !String.IsNullOrWhiteSpace(hostAndPort[0]))
            {
                var parts = hostAndPort[0].Split(':');
                host = parts[0];
                if (parts.Length > 1)
                {
                    int.TryParse(parts[1], out port);
                }
            }

            var uriBuilder = new UriBuilder(requestScheme, host, port, requestPathBase + requestPath);
            return uriBuilder;
        }

        public string ToFullUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute)) return url;

            if (!url.StartsWith("/"))
            {
                return "/" + url;
            }

            var requestScheme = get<string>(OwinConstants.RequestSchemeKey) + "://";
            if (url.StartsWith(requestScheme, StringComparison.OrdinalIgnoreCase)) return url;

            return uriBuilderFor(url).Uri.ToString();
        }

        public string HttpMethod()
        {
            return get<string>(OwinConstants.RequestMethodKey);
        }

        public OwinCurrentHttpRequest HttpMethod(string method)
        {
            append(OwinConstants.RequestMethodKey, method);
            return this;
        }

        public OwinCurrentHttpRequest Header(string key, params string[] values)
        {
            values.Each(x => _headers.Value.AppendValue(key, x));

            return this;
        }

        public OwinCurrentHttpRequest IfNoneMatch(string etag)
        {
            return Header(HttpRequestHeaders.IfNoneMatch, etag);
        }

        public OwinCurrentHttpRequest IfMatch(string etag)
        {
            return Header(HttpRequestHeaders.IfMatch, etag);
        }

        public OwinCurrentHttpRequest IfModifiedSince(DateTime time)
        {
            return Header(HttpRequestHeaders.IfModifiedSince, time.ToUniversalTime().ToString("r"));
        }

        public OwinCurrentHttpRequest IfUnModifiedSince(DateTime time)
        {
            return Header(HttpRequestHeaders.IfUnmodifiedSince, time.ToUniversalTime().ToString("r"));
        }

        public bool HasHeader(string key)
        {
            return _headers.Value.ContainsKey(key) || _headers.Value.Keys.Any(x => x.EqualsIgnoreCase(key));
        }

        public IEnumerable<string> GetHeader(string key)
        {
            if (!HasHeader(key)) return new string[0];

            key = _headers.Value.Keys.FirstOrDefault(x => x.EqualsIgnoreCase(key));
            return key.IsEmpty() ? new string[0] : _headers.Value.Get(key);
        }

        public IEnumerable<string> AllHeaderKeys()
        {
            return _headers.Value.Keys;
        }

        public NameValueCollection QueryString
        {
            get
            {
                return _querystring.Value;
            }
            
        }
    }

}