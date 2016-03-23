using System.Security.Cryptography.X509Certificates;
using FubuMVC.Core.Security.Authentication.Saml2.Validation;

namespace FubuMVC.Core.Security.Authentication.Saml2.Certificates
{
    public interface ICertificateService
    {
        SamlValidationKeys Validate(SamlResponse response);
        X509Certificate2 LoadCertificate(string issuer);
    }
}