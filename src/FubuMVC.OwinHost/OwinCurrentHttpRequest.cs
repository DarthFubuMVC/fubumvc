using System;
using System.Collections.Generic;
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

        public OwinCurrentHttpRequest(IDictionary<string, object> environment)
        {
            _environment = environment;
            _headers = new Lazy<IDictionary<string, string[]>>(() => {
                return environment.Get<IDictionary<string, string[]>>(OwinConstants.RequestHeadersKey);
            });
        }

        private T Get<T>(string key)
        {
            object value;
            return _environment.TryGetValue(key, out value) ? (T)value : default(T);
        }

        public string RawUrl()
        {
            var queryString = Get<string>(OwinConstants.RequestQueryStringKey);
            if (string.IsNullOrEmpty(queryString))
            {
                return Get<string>(OwinConstants.RequestPathBaseKey) + Get<string>(OwinConstants.RequestPathKey);
            }
            return Get<string>("owin.RequestPathBase") + Get<string>("owin.RequestPath") + "?" + Get<string>("owin.RequestQueryString");
        }

        public string RelativeUrl()
        {
            return _environment.Get<string>(OwinConstants.RequestPathKey);
        }

        public string FullUrl()
        {
            var requestPath = Get<string>(OwinConstants.RequestPathKey);


            var uriBuilder = uriBuilderFor(requestPath);


            var requestQueryString = Get<string>(OwinConstants.RequestQueryStringKey);
            if (!String.IsNullOrEmpty(requestQueryString))
            {
                uriBuilder.Query = requestQueryString;
            }
            return uriBuilder.Uri.ToString();
        }

        private UriBuilder uriBuilderFor(string requestPath)
        {
            var requestScheme = Get<string>(OwinConstants.RequestSchemeKey);
            var requestPathBase = Get<string>(OwinConstants.RequestPathBaseKey);

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
            var requestScheme = Get<string>(OwinConstants.RequestSchemeKey) + "://";
            if (url.StartsWith(requestScheme, StringComparison.OrdinalIgnoreCase)) return url;

            return uriBuilderFor(url).Uri.ToString();
        }

        public string HttpMethod()
        {
            return Get<string>(OwinConstants.RequestMethodKey);
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
    }
}