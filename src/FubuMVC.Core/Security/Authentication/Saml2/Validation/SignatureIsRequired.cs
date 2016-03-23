namespace FubuMVC.Core.Security.Authentication.Saml2.Validation
{
    public class SignatureIsRequired : ISamlValidationRule
    {
        public void Validate(SamlResponse response)
        {
            if (response.Signed != SignatureStatus.Signed)
            {
                response.LogError(new SamlError(response.Signed));
            }
        }
    }
}