using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core.Security.Authentication
{
    public class AuthenticationIsConfigured : IActivator
    {
        private readonly IEnumerable<IAuthenticationStrategy> _strategies;

        public AuthenticationIsConfigured(IEnumerable<IAuthenticationStrategy> strategies)
        {
            _strategies = strategies;
        }

        public void Activate(IActivationLog log, IPerfTimer timer)
        {
            if (!_strategies.Any())
            {
                log.MarkFailure("There are no IAuthenticationStrategy services registered.  Either register an IAuthenticationStrategy or remove FubuMVC.Authentication from your application");
            }
        }
    }
}