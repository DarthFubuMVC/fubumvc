using System.Linq;
using System.Web;
using FubuCore.Dates;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http.Cookies
{
    // TODO -- This is a great big bag of fail for testing
    public class CookieValue : ICookieValue
    {
        private readonly string _cookieName;
        private readonly ICookies _cookies;
        private readonly ISystemTime _time;
        private readonly IOutputWriter _writer;

        public CookieValue(string cookieName, ISystemTime time, ICookies cookies, IOutputWriter writer)
        {
            _time = time;
            _cookies = cookies;
            _writer = writer;
            _cookieName = cookieName;
        }

        public string Value
        {
            get
            {
                var cookie = _cookies.All.SingleOrDefault(x => x.Matches(_cookieName));
                return cookie == null ? null : cookie.For(_cookieName).Value;
            }
            set
            {
                var cookie = new Cookie(_cookieName, value)
                {
                    Expires = _time.UtcNow().AddYears(1)
                };

                _writer.AppendCookie(cookie);
            }
        }

        public void Erase()
        {
            var cookie = new Cookie(_cookieName, "BLANK")
            {
                Expires = _time.UtcNow().AddYears(-1)
            };

            _writer.AppendCookie(cookie);
        }
    }
}