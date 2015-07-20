using FubuCore.Dates;
using FubuMVC.Core.Http.Cookies;

namespace FubuMVC.Core.Security.Authentication.Cookies
{
    public interface ILoginCookieService
    {
        Cookie Current();
        Cookie CreateCookie(ISystemTime clock);

        void Update(Cookie cookie);
    }
}