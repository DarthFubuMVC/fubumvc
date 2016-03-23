using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Security.Authentication.Saml2.Validation
{
    /// <summary>
    /// The Saml2 spec considers the audience to be "Any", so we do too
    /// </summary>
    public class AudienceValidationRule : ISamlValidationRule
    {
        private readonly string[] _audiences;

        public AudienceValidationRule(string audience)
        {
            _audiences = new []{audience};
        }

        public AudienceValidationRule(params string[] audiences)
        {
            _audiences = audiences;
        }

        public IEnumerable<string> Audiences
        {
            get { return _audiences; }
        }

        public void Validate(SamlResponse response)
        {
            var restrictions = response.AudienceRestrictions;
            if (!restrictions.Any()) return;

            if (!_audiences.Any(x => restrictions.Any(r => r.Audiences.Contains(x))))
            {
                response.LogError(new SamlError(SamlValidationKeys.AudiencesDoNotMatch));
            }
        }
    }
}