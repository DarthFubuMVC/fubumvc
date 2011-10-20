using System;
using System.Web;
using System.Web.Routing;
using FubuCore.Binding;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetServiceArguments : ServiceArguments
    {
        public AspNetServiceArguments(RequestContext requestContext)
        {
            With<AggregateDictionary>(new AspNetAggregateDictionary(requestContext));
            With(requestContext.HttpContext);

            With<ICurrentRequest>(new AspNetCurrentRequest(requestContext.HttpContext.Request));
        }
    }

    // Tested manually against FubuTestApplication
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