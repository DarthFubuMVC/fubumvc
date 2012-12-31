using System;
using System.Collections.Generic;
using System.Web;
using FubuCore;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost
{

    public class OwinCurrentHttpRequest : ICurrentHttpRequest
    {
        private readonly IDictionary<string, object> _environment;

        public OwinCurrentHttpRequest(IDictionary<string, object> environment)
        {
            _environment = environment;
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
            var requestHeaders = Get<IDictionary<string, string[]>>(OwinConstants.RequestHeadersKey);
            var requestScheme = Get<string>(OwinConstants.RequestSchemeKey);
            var requestPathBase = Get<string>(OwinConstants.RequestPathBaseKey);
            var requestPath = Get<string>(OwinConstants.RequestPathKey);
            var requestQueryString = Get<string>(OwinConstants.RequestQueryStringKey);

            // default values, in absence of a host header
            string host = "127.0.0.1";
            int port = String.Equals(requestScheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ? 443 : 80;

            // if a single host header is available
            string[] hostAndPort;
            if (requestHeaders.TryGetValue("Host", out hostAndPort) &&
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
            if (!String.IsNullOrEmpty(requestQueryString))
            {
                uriBuilder.Query = requestQueryString;
            }
            return uriBuilder.Uri.ToString();
        }

        public string ToFullUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute)) return url;
            if (url.IsEmpty()) { url = "~/"; }

            var urlParts = url.Split(new[] { '?' }, 2);
            var baseUrl = urlParts[0];

            if (!VirtualPathUtility.IsAbsolute(baseUrl))
            {
                baseUrl = VirtualPathUtility.Combine("~", baseUrl);
            }

            var absoluteUrl = VirtualPathUtility.ToAbsolute(baseUrl);

            if (urlParts.Length > 1) absoluteUrl += ("?" + urlParts[1]);
            return absoluteUrl;
        }

        public string HttpMethod()
        {
            return Get<string>(OwinConstants.RequestMethodKey);
        }
    }
}