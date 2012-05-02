using System;
using FubuCore;
using FubuMVC.Core;
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

        public string FullUrl()
        {
            var parts = _body.HostWithPort.Split(':');
            var builder = new UriBuilder(_body.Environment.Scheme, parts[0]);
            if (parts.Length > 1 && parts[1].Trim().IsNotEmpty())
            {
                var port = 0;
                if (Int32.TryParse(parts[1], out port))
                {
                    builder.Port = port;
                }
            }
            builder.Path = _body.Path;
            builder.Query = _body.Environment.QueryString;
            return builder.ToString();
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