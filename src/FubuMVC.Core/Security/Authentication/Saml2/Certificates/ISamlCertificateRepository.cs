using System.Collections.Generic;

namespace FubuMVC.Core.Security.Authentication.Saml2.Certificates
{
    public interface ISamlCertificateRepository
    {
        SamlCertificate Find(string samlIssuer);
        IEnumerable<SamlCertificate> AllKnownCertificates();
    }
}