using System.Web;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Runtime;

namespace AspNetApplication
{
    public class CookieEndpoint
    {
        public const string CookieName = "Test";

        private readonly ICookies _cookies;
        private readonly IOutputWriter _writer;

        public CookieEndpoint(ICookies cookies, IOutputWriter writer)
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
            _writer.AppendCookie(new Cookie(request.Name, request.Value));
        }
    }

    public class CookieInfo { }
    public class WriteCookieRequest
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}