using FubuCore.Dates;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Security.Authentication.Cookies
{
    public class BasicFubuLoginCookies : ILoginCookies
    {
        public static readonly string FubuRemember = "FubuRememberMe";

        private readonly ICookies _cookies;
        private readonly ISystemTime _time;
        private readonly IOutputWriter _writer;

        public BasicFubuLoginCookies(ISystemTime time, ICookies cookies, IOutputWriter writer)
        {
            _time = time;
            _cookies = cookies;
            _writer = writer;
        }

        public ICookieValue User
        {
            get { return new CookieValue(FubuRemember, _time, _cookies, _writer); }
        }
    }
}