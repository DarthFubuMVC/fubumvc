using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Logging;
using FubuMVC.Core.Security.Authentication.Saml2.Encryption;
using FubuMVC.Core.Security.Authentication.Saml2.Validation;

namespace FubuMVC.Core.Security.Authentication.Saml2
{
    public class SamlAuthenticationStrategy : IAuthenticationStrategy
    {
        public const string SamlResponseKey = "SAMLResponse";
        public static readonly AuthResult DoesNotApply = new AuthResult{Success = false};

        private readonly IRequestData _requestData;
        private readonly ISamlDirector _director;
        private readonly ISamlResponseReader _reader;
        private readonly IEnumerable<ISamlValidationRule> _rules;
        private readonly ILogger _logger;
        private readonly IEnumerable<ISamlResponseHandler> _strategies;

        public SamlAuthenticationStrategy(IRequestData requestData, ISamlDirector director, ISamlResponseReader reader, IEnumerable<ISamlValidationRule> rules, ILogger logger, IEnumerable<ISamlResponseHandler> strategies)
        {
            _requestData = requestData;
            _director = director;
            _reader = reader;
            _rules = rules;
            _logger = logger;
            _strategies = strategies;
        }

        public AuthResult TryToApply()
        {
            AuthResult result = AuthResult.Failed();

            _requestData.Value(SamlResponseKey, v => {
                try
                {
                    var xml = v.RawValue as string;
                    ProcessSamlResponseXml(xml);
                }
                catch (Exception e)
                {
                    _logger.Error("Saml Response handling failed", e);
                    _director.FailedUser();
                }

                result = _director.Result();
            });

            return result;
        }

        public virtual void ProcessSamlResponseXml(string xml)
        {
            var response = _reader.Read(xml);

            _rules.Each(x => x.Validate(response));

            var handler = _strategies.FirstOrDefault(x => x.CanHandle(response));
            if (handler == null)
            {
                _logger.InfoMessage(() => new SamlAuthenticationFailed(response));
                _director.FailedUser();
            }
            else
            {
                handler.Handle(_director, response);
            }
        }

        public bool Authenticate(LoginRequest request)
        {
            return false;
        }
    }


}