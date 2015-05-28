using FubuCore;
using FubuMVC.Core.Continuations;

namespace FubuMVC.Authentication
{
    public class LoginSuccessHandler : ILoginSuccessHandler
    {
        public FubuContinuation LoggedIn(LoginRequest request)
        {
            string url = request.Url;
            if (url.IsEmpty())
            {
                url = "~/";
            }

            return FubuContinuation.RedirectTo(url);
        }
    }
}