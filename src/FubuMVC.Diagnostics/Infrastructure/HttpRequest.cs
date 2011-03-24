using System.Web;

namespace FubuMVC.Diagnostics.Infrastructure
{
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