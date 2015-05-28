using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Cookies;

namespace FubuMVC.Authentication.Cookies
{
    public interface ILoginCookies
    {
        ICookieValue User { get; }
    }
}