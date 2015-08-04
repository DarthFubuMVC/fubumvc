using FubuCore;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Security.Authentication.Cookies;
using FubuMVC.Core.Security.Authorization;

namespace FubuMVC.Core.Security.Authentication.Endpoints
{
    public class LoginController
    {
        private readonly ILoginCookies _cookies;
        private readonly ILoginSuccessHandler _handler;
        private readonly ILockedOutRule _lockedOutRule;
        private readonly IAuthenticationService _service;

        public LoginController(ILoginCookies cookies, IAuthenticationService service, ILoginSuccessHandler handler,
                               ILockedOutRule lockedOutRule)
        {
            _cookies = cookies;
            _service = service;
            _handler = handler;
            _lockedOutRule = lockedOutRule;
        }

        [NotAuthenticated]
        public FubuContinuation post_login(LoginRequest request)
        {
            bool authenticated = _service.Authenticate(request);

            _service.SetRememberMeCookie(request);

            if (authenticated)
            {
                return _handler.LoggedIn(request);
            }

            return FubuContinuation.TransferTo(request, "GET");
        }

        [NotAuthenticated]
        public LoginRequest get_login(LoginRequest request)
        {
            if (request.Status == LoginStatus.NotAuthenticated)
            {
                request.Status = _lockedOutRule.IsLockedOut(request);
            }

            _service.SetRememberMeCookie(request);

            if (request.UserName.IsEmpty())
            {
                string remembered = _cookies.User.Value;

                if (remembered.IsNotEmpty())
                {
                    request.UserName = remembered;
                    request.RememberMe = true;
                }
            }

            if (request.Status == LoginStatus.LockedOut)
            {
                request.Message = LoginKeys.LockedOut.ToString();
            }
            else if (request.Status == LoginStatus.Failed && request.Message.IsEmpty())
            {
                request.Message = LoginKeys.Unknown.ToString();
            }

            return request;
        }
    }
}