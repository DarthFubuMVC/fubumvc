using System.Web;
using FubuCore;

namespace FubuMVC.Diagnostics.Core.Infrastructure
{
    [MarkedForTermination("Need to get rid of this")]
    public class HttpRequest : IHttpRequest
    {
        private readonly HttpContextBase _httpContext;

        public HttpRequest(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }

        public string CurrentUrl()
        {
            var url = _httpContext.Request.RawUrl;
			if (url.EndsWith("/"))
			{
				url = url.TrimEnd('/');
			}

        	return url;
        }
    }
}