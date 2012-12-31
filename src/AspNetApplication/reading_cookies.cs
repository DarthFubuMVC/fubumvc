using System.Web;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Cookies;

namespace AspNetApplication
{
    public class CookieEndpoint
    {
        public const string CookieName = "Test";

        private readonly ICookies _cookies;
        private readonly IHttpWriter _writer;

        public CookieEndpoint(ICookies cookies, IHttpWriter writer)
        {
            _cookies = cookies;
            _writer = writer;
        }

        public string get_cookie_info(CookieInfo info)
        {
            return _cookies.Get(CookieName).Value;
        }

        public void post_write_cookie(WriteCookieRequest request)
        {
            _writer.AppendCookie(new HttpCookie(request.Name, request.Value));
        }
    }

    public class CookieInfo { }
    public class WriteCookieRequest
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}