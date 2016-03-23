using System.Linq;
using FubuCore.Logging;
using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Security.Authentication.Saml2
{
    /// <summary>
    /// Abstract class to implement for basic saml response handling
    /// </summary>
    public abstract class BasicSamlResponseHandler : ISamlResponseHandler
    {
        private readonly ILogger _logger;

        protected BasicSamlResponseHandler(ILogger logger)
        {
            _logger = logger;
        }

        public abstract bool CanHandle(SamlResponse response);


        public void Handle(ISamlDirector director, SamlResponse response)
        {
            validate(response);
            if (response.Errors.Any())
            {
                _logger.InfoMessage(() => new SamlAuthenticationFailed(response));
                director.FailedUser(failedContinuation(response)); // just let it go to the login page
            }
            else
            {
                _logger.InfoMessage(() => new SamlAuthenticationSucceeded(response));
                var persistedUsername = createLocalUser(response);
                director.SuccessfulUser(persistedUsername, successfulContinuation(response));
            }
        }

        protected virtual FubuContinuation successfulContinuation(SamlResponse response)
        {
            return null;
        }

        protected virtual FubuContinuation failedContinuation(SamlResponse response)
        {
            return null;
        }

        protected abstract string createLocalUser(SamlResponse response);
        protected virtual void validate(SamlResponse response)
        {
            // Nothing
        }
    }
}