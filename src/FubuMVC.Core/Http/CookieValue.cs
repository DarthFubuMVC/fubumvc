using System.Web;
using FubuCore.Dates;

namespace FubuMVC.Core.Http
{
    // TODO -- This is a great big bag of fail for testing
    public class CookieValue : ICookieValue
    {
        private readonly string _cookieName;
        private readonly ICookies _cookies;
        private readonly ISystemTime _time;

        public CookieValue(string cookieName, ISystemTime time, ICookies cookies)
        {
            _time = time;
            _cookies = cookies;
            _cookieName = cookieName;
        }

        public string Value
        {
            get
            {
                var cookie = _cookies.Request[_cookieName];
                return cookie == null ? null : cookie.Value;
            }
            set
            {
                var cookie = new HttpCookie(_cookieName)
                {
                    Value = value,
                    Expires = _time.UtcNow().AddYears(1)
                };

                _cookies.Response.Add(cookie);
            }
        }

        public void Erase()
        {
            var cookie = new HttpCookie(_cookieName)
            {
                Expires = _time.UtcNow().AddYears(-1)
            };

            _cookies.Response.Add(cookie);
        }
    }
}