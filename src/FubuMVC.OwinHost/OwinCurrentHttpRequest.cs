using System;
using FubuCore;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost
{
    public class OwinCurrentHttpRequest : ICurrentHttpRequest
    {
        private readonly Lazy<string> _baseUrl;
        private OwinRequestBody _body;

        public OwinCurrentHttpRequest(OwinRequestBody body)
        {
            _body = body;

            // TODO -- Owin and protocol?
            _baseUrl = new Lazy<string>(() => "http://" + _body.HostWithPort + "/" + _body.PathBase.TrimEnd('/'));
        }

        public string RawUrl()
        {
            return ToFullUrl(_body.Path);
        }

        public string RelativeUrl()
        {
            return _body.Path;
        }

        public string ToFullUrl(string url)
        {
            return url.ToAbsoluteUrl(_baseUrl.Value);
        }

        public string HttpMethod()
        {
            return _body.Method;
        }
    }
}