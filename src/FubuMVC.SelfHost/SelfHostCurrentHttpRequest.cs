using System;
using System.Net.Http;
using FubuMVC.Core;
using FubuMVC.Core.Http;

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
    }
}