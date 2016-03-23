using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Security.Authentication.Saml2.Validation
{
    public class SamlValidationKeys : StringToken
    {
        public static readonly SamlValidationKeys TimeFrameDoesNotMatch = new SamlValidationKeys("Outside the valid time frame of this Saml Response");
        public static readonly SamlValidationKeys CannotMatchIssuer = new SamlValidationKeys("The certificate does not match the issuer");
        public static readonly SamlValidationKeys NoValidCertificates = new SamlValidationKeys("No valid certificates were found");
        public static readonly SamlValidationKeys ValidCertificate = new SamlValidationKeys("The certificate was valid");
        public static readonly SamlValidationKeys AudiencesDoNotMatch = new SamlValidationKeys("Invalid audience for this application");
        public static readonly SamlValidationKeys UnableToValidationSamlResponse = new SamlValidationKeys("Unable to authenticate this Saml response");


        protected SamlValidationKeys(string defaultValue) : base(null, defaultValue, namespaceByType:true)
        {
        }
    }
}