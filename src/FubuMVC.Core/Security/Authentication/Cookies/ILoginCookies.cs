using FubuMVC.Core.Http.Cookies;

namespace FubuMVC.Core.Security.Authentication.Cookies
{
    public interface ILoginCookies
    {
        ICookieValue User { get; }
    }
}