using System.Web;

namespace FubuMVC.Diagnostics.Infrastructure
{
    public class HttpRequest : IHttpRequest
    {
        public string CurrentUrl()
        {
            return HttpContext.Current.Request.RawUrl;
        }
    }
}