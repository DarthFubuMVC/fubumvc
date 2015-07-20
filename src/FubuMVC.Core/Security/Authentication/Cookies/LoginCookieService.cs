using FubuCore;
using FubuCore.Dates;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security.Authentication.Cookies
{
    public class LoginCookieService : ILoginCookieService
    {
        private readonly CookieSettings _settings;
        private readonly ICookies _cookies;
        private readonly IOutputWriter _writer;

        public LoginCookieService(CookieSettings settings, ICookies cookies, IOutputWriter writer)
        {
            _settings = settings;
            _cookies = cookies;
            _writer = writer;
        }

        public Cookie Current()
        {
            return _cookies.Get(_settings.Name);
        }

        public Cookie CreateCookie(ISystemTime clock)
        {
            var cookie = new Cookie(_settings.Name)
            {
                HttpOnly = _settings.HttpOnly,
                Secure = _settings.Secure,
                Domain = _settings.Domain
            };

            if(_settings.Path.IsNotEmpty())
            {
                cookie.Path = _settings.Path;
            }

            cookie.Expires = _settings.ExpirationFor(clock.UtcNow());

            return cookie;
        }

        public void Update(Cookie cookie)
        {
            _writer.AppendCookie(cookie);
        }
    }
}