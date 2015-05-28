using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.Authentication
{
    public class AuthenticationIsConfigured : IActivator
    {
        private readonly IEnumerable<IAuthenticationStrategy> _strategies;

        public AuthenticationIsConfigured(IEnumerable<IAuthenticationStrategy> strategies)
        {
            _strategies = strategies;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            if (!_strategies.Any())
            {
                log.MarkFailure("There are no IAuthenticationStrategy services registered.  Either register an IAuthenticationStrategy or remove FubuMVC.Authentication from your application");
            }
        }
    }
}