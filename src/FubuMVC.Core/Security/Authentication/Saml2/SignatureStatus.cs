using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Security.Authentication.Saml2
{
    public class SignatureStatus : StringToken
    {
        public static readonly SignatureStatus NotSigned = new SignatureStatus("The SamlResponse was not signed");
        public static readonly SignatureStatus InvalidSignature = new SignatureStatus("The SamlResponse signature was invalid");
        public static readonly SignatureStatus Signed = new SignatureStatus("The SamlResponse signature is valid");
        

        protected SignatureStatus(string defaultValue) : base(null, defaultValue, namespaceByType:true)
        {
        }
    }
}