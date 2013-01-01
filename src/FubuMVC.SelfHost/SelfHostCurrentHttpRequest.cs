using System;
using System.Collections.Generic;
using System.Net.Http;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using System.Linq;
using FubuCore;

namespace FubuMVC.SelfHost
{
    public class SelfHostCurrentHttpRequest : ICurrentHttpRequest
    {
        private readonly HttpRequestMessage _request;

        public SelfHostCurrentHttpRequest(HttpRequestMessage request)
        {
            _request = request;
        }

        public string RawUrl()
        {
            return _request.RequestUri.OriginalString;
        }

        public string RelativeUrl()
        {
            return _request.RequestUri.AbsolutePath.TrimStart('/');
        }

        public string FullUrl()
        {
            return ToFullUrl(RelativeUrl());
        }

        public string ToFullUrl(string url)
        {
            return url.ToAbsoluteUrl(_request.RequestUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped));
        }

        public string HttpMethod()
        {
            return _request.Method.Method;
        }

        public bool HasHeader(string key)
        {
            return allHeaders().Any(x => x.Key.EqualsIgnoreCase(key));
        }

        public IEnumerable<string> GetHeader(string key)
        {
            if (!HasHeader(key))
            {
                return new string[0];
            }

            return allHeaders().FirstOrDefault(x => x.Key.EqualsIgnoreCase(key)).Value;
        }

        public IEnumerable<string> AllHeaderKeys()
        {
            return allHeaders().Select(x => x.Key).Distinct().ToArray();
        }

        private IEnumerable<KeyValuePair<string, IEnumerable<string>>> allHeaders()
        {
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers = _request.Headers;
            if (_request.Content != null)
            {
                headers = headers.Union(_request.Content.Headers);
            }


            return headers;
        }
    }
}