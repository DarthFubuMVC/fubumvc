using System;
using System.Collections.Generic;
using System.Net.Http;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using System.Linq;

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
            return _request.Headers.Contains(key);
        }

        public IEnumerable<string> GetHeader(string key)
        {
            return _request.Headers.GetValues(key);
        }

        public IEnumerable<string> AllHeaderKeys()
        {
            return _request.Headers.Select(x => x.Key).Distinct().ToArray();
        }
    }
}