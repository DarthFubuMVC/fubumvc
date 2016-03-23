namespace FubuMVC.Core.Security.Authentication.Saml2.Validation
{
    public interface ISamlValidationRule
    {
        void Validate(SamlResponse response);
    }
}