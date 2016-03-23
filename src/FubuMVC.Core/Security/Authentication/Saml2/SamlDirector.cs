using FubuMVC.Core.Continuations;
using FubuMVC.Core.Security.Authentication.Saml2.Validation;

namespace FubuMVC.Core.Security.Authentication.Saml2
{
    public class SamlDirector : ISamlDirector
    {
        private readonly IPrincipalBuilder _principalBuilder;
        private readonly IAuthenticationSession _session;
        private readonly IPrincipalContext _context;
        private readonly AuthResult _result;


        public SamlDirector(IPrincipalBuilder principalBuilder, IAuthenticationSession session, IPrincipalContext context)
        {
            _principalBuilder = principalBuilder;
            _session = session;
            _context = context;
            _result = new AuthResult
            {
                Success = false,
                Continuation =
                    FubuContinuation.RedirectTo(new LoginRequest
                    {
                        Message = SamlValidationKeys.UnableToValidationSamlResponse
                    }, "GET")
            };
        }

        public void SuccessfulUser(string username, FubuContinuation redirection = null)
        {
            _result.Success = true;
            _result.Continuation = redirection ?? FubuContinuation.NextBehavior();

            var principal = _principalBuilder.Build(username);
            _context.Current = principal;
            _session.MarkAuthenticated(username);
        }

        public void FailedUser(FubuContinuation redirection = null)
        {
            _result.Success = false;
            if (redirection != null) _result.Continuation = redirection;
        }

        public AuthResult Result()
        {
            return _result;
        }


    }
}