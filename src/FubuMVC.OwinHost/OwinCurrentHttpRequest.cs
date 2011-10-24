using FubuCore;
using FubuMVC.Core.Http;
using Gate.Helpers;

namespace FubuMVC.OwinHost
{
    public class OwinCurrentHttpRequest : ICurrentHttpRequest
    {
        private readonly Request _request;

        public OwinCurrentHttpRequest(Request request)
        {
            _request = request;
        }

        public string RawUrl()
        {
            return _request.Path.ToAbsoluteUrl(_request.PathBase);
        }

        public string RelativeUrl()
        {
            return _request.Path;
        }

        public string ApplicationRoot()
        {
            // TODO move to a lazy
            return _request.HostWithPort + "/" + _request.PathBase.TrimEnd('/');
        }

        public string HttpMethod()
        {
            return _request.Method;
        }
    }
}