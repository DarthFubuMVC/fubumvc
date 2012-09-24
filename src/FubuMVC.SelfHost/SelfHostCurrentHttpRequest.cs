using System;
using System.Net.Http;
using System.Web.Http.SelfHost;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuCore;

namespace FubuMVC.SelfHost
{
    public class SelfHostCurrentHttpRequest : ICurrentHttpRequest
    {
        private readonly HttpRequestMessage _request;
        private readonly HttpSelfHostConfiguration _configuration;

        public SelfHostCurrentHttpRequest(HttpRequestMessage request, HttpSelfHostConfiguration configuration)
        {
            _request = request;
            _configuration = configuration;
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
            return url.ToAbsoluteUrl(_configuration.BaseAddress.OriginalString);
        }

        public string HttpMethod()
        {
            return _request.Method.Method;
        }
    }
}