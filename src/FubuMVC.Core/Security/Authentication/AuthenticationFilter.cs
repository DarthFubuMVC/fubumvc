using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;
using FubuMVC.Core.Security.Authorization;

namespace FubuMVC.Core.Security.Authentication
{
    public class AuthenticationFilter
    {
        private readonly IAuthenticationService _authentication;
        private readonly ICurrentChain _currentChain;
        private readonly SecuritySettings _settings;
        private readonly IAuthenticationRedirector _redirector;

        public AuthenticationFilter(IAuthenticationRedirector redirector, IAuthenticationService authentication, ICurrentChain currentChain, SecuritySettings settings)
        {
            _redirector = redirector;
            _authentication = authentication;
            _currentChain = currentChain;
            _settings = settings;
        }

        public FubuContinuation Authenticate()
        {
            if (!_settings.AuthenticationEnabled) return FubuContinuation.NextBehavior();

            if (_currentChain.IsInPartial()) return FubuContinuation.NextBehavior();

            var result = _authentication.TryToApply();
            if (result.Continuation != null) return result.Continuation;

            return result.Success ? FubuContinuation.NextBehavior() : _redirector.Redirect();
        }
    }
}