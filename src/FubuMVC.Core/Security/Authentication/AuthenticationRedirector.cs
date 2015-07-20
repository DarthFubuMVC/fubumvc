using System.Linq;
using FubuCore;
using FubuMVC.Core.Continuations;

namespace FubuMVC.Core.Security.Authentication
{
    public class AuthenticationRedirector : IAuthenticationRedirector
    {
        private readonly RedirectLibrary _library;
        private readonly IServiceLocator _services;

        public AuthenticationRedirector(RedirectLibrary library, IServiceLocator services)
        {
            _library = library;
            _services = services;
        }

        public FubuContinuation Redirect()
        {
            var redirector = _library.GetRedirectTypes()
                                     .Select(x =>  _services.GetInstance(x).As<IAuthenticationRedirect>())
                                     .FirstOrDefault(x => x.Applies());

            return redirector.Redirect();
        }
    }
}