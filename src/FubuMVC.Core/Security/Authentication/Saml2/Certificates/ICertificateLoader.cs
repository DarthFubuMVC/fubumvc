using System.Security.Cryptography.X509Certificates;

namespace FubuMVC.Core.Security.Authentication.Saml2.Certificates
{
    public interface ICertificateLoader
    {
        X509Certificate2 Load(string thumbprint);
    }
}