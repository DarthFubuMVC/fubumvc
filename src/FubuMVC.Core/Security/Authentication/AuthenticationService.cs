using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuMVC.Core.Security.Authentication.Auditing;
using FubuMVC.Core.Security.Authentication.Cookies;

namespace FubuMVC.Core.Security.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IAuthenticationStrategy> _strategies;
        private readonly ILoginAuditor _auditor;
        private readonly ILoginCookies _cookies;

        public AuthenticationService(
            ILogger logger,
            IEnumerable<IAuthenticationStrategy> strategies,
            ILoginAuditor auditor,
            ILoginCookies cookies)
        {
            if (!strategies.Any())
            {
                throw new ArgumentOutOfRangeException("strategies", "There must be at least one IAuthenticationStrategy");
            }

            _logger = logger;
            _strategies = strategies;
            _auditor = auditor;
            _cookies = cookies;
        }

        public AuthResult TryToApply()
        {
            foreach (var strategy in _strategies)
            {
                var result = strategy.TryToApply();
                _logger.DebugMessage(new TriedToApplyStrategy { Result = result, Strategy = strategy });
                
                if (result.IsDeterministic())
                {
                    return result;
                }
            }

            _logger.DebugMessage(new NonDeterministicResult());
            return new AuthResult{Success = false};
        }

        public bool Authenticate(LoginRequest request)
        {
            return Authenticate(request, _strategies);
        }

        public bool Authenticate(LoginRequest request, IEnumerable<IAuthenticationStrategy> strategies)
        {
            _auditor.ApplyHistory(request);
            var authResult = strategies.Any(x => x.Authenticate(request));
            _auditor.Audit(request);
            return authResult;
        }

        public void SetRememberMeCookie(LoginRequest request)
        {
            if (request.RememberMe && request.UserName.IsNotEmpty())
            {
                _cookies.User.Value = request.UserName;
            }
        }
    }

    public class NonDeterministicResult : LogTopic
    {
        public override string ToString()
        {
            return "No authentication strategies were able to make a deterministic authentication result";
        }
    }

    public class TriedToApplyStrategy : LogTopic, DescribesItself
    {
        public AuthResult Result { get; set; }
        public IAuthenticationStrategy Strategy { get; set; }

        public override string ToString()
        {
            return "Authentication returned {0}".ToFormat(Result);
        }

        public void Describe(Description description)
        {
            description.Title = "Trying to apply strategy {0}".ToFormat(Strategy);
            description.ShortDescription = ToString();
        }
    }
}