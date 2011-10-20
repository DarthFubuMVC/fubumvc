using System.Web;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetCurrentRequest : ICurrentRequest
    {
        private readonly HttpRequestBase _request;

        public AspNetCurrentRequest(HttpRequestBase request)
        {
            _request = request;
        }

        public string RawUrl()
        {
            return _request.RawUrl;
        }

        public string RelativeUrl()
        {
            return _request.PathInfo;
        }

        public string ApplicationRoot()
        {
            return _request.ApplicationPath.TrimEnd('/');
        }

        public string HttpMethod()
        {
            return _request.HttpMethod;
        }
    }
}