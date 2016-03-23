using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Security.Authentication.Saml2.Certificates;

namespace FubuMVC.Core.Security.Authentication.Saml2
{
    public class SamlResponse
    {
        private readonly IDictionary<string, object> _attributes =
            new Dictionary<string, object>();

        private readonly IList<SamlError> _errors = new List<SamlError>(); 

        public SamlResponse()
        {
            Attributes = new DictionaryKeyValues<object>(_attributes);
            Subject = new Subject();
        }

        public void LogError(SamlError error)
        {
            _errors.Add(error);
        }

        public void LogError(StringToken message)
        {
            _errors.Add(new SamlError(message));
        }

        public IEnumerable<SamlError> Errors
        {
            get { return _errors; }
        } 

        public IEnumerable<AudienceRestriction> AudienceRestrictions
        {
            get
            {
                if (Conditions == null) return new AudienceRestriction[0];

                return Conditions.Conditions.OfType<AudienceRestriction>();
            }
        } 

        public SamlStatus Status { get; set; }
        public string Issuer { get; set; }
        public IEnumerable<ICertificate> Certificates { get; set; }
        public SignatureStatus Signed { get; set; }

        public AuthenticationStatement Authentication { get; set; }

        public Uri Destination { get; set; }
        public string Id { get; set; }
        public DateTimeOffset IssueInstant { get; set; }

        public Subject Subject { get; set; }

        // valid if no conditions
        public ConditionGroup Conditions { get; set; }

        public IKeyValues<object> Attributes { get; private set; }

        public void AddAttribute(string key, string value)
        {
            if (_attributes.ContainsKey(key))
            {
                object existing = _attributes[key];
                if (existing is string)
                {
                    var list = new List<string> {existing as string, value};
                    _attributes[key] = list;
                }

                else
                {
                    existing.As<IList<string>>().Add(value);
                }
            }
            else
            {
                _attributes.Add(key, value);
            }
        }

        public void AddAudienceRestriction(string audience)
        {
            if (Conditions == null)
            {
                Conditions = new ConditionGroup();
            }

            var restriction = Conditions.Conditions.OfType<AudienceRestriction>().FirstOrDefault();
            if (restriction == null)
            {
                restriction = new AudienceRestriction();
                Conditions.Add(restriction);
            }

            restriction.Add(audience);
        }
    }
}