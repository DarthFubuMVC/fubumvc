using System.Web;

namespace FubuMVC.Diagnostics.Core.Infrastructure
{
    // TODO -- change this to ICurrentRequest?
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