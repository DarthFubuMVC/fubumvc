using System;
using FubuCore;
using FubuMVC.Core.Http;
using Gate.Helpers;

namespace FubuMVC.OwinHost
{
    public class OwinCurrentHttpRequest : ICurrentHttpRequest
    {
        private readonly Request _request;
        private readonly Lazy<string> _baseUrl;

        public OwinCurrentHttpRequest(Request request)
        {
            _request = request;

            // TODO -- Owin and protocol?
            _baseUrl = new Lazy<string>(() => "http://" + _request.HostWithPort + "/" + _request.PathBase.TrimEnd('/'));
        }

        public string RawUrl()
        {
            return _request.Path.ToAbsoluteUrl(_request.PathBase);
        }

        public string RelativeUrl()
        {
            return _request.Path;
        }

        public string ToFullUrl(string url)
        {
            return url.ToAbsoluteUrl(_baseUrl.Value);
        }

        public string HttpMethod()
        {
            return _request.Method;
        }
    }
}