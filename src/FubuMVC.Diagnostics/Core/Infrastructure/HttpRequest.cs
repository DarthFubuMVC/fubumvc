using System.Web;
using FubuCore;

namespace FubuMVC.Diagnostics.Core.Infrastructure
{
    [MarkedForTermination("Need to get rid of this")]
    public class HttpRequest : IHttpRequest
    {
        public string CurrentUrl()
        {
            var url = HttpContext.Current.Request.RawUrl;
			if (url.EndsWith("/"))
			{
				url = url.TrimEnd('/');
			}

        	return url;
        }
    }
}