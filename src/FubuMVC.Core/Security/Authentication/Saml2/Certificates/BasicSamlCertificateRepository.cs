using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Security.Authentication.Saml2.Certificates
{
    public class BasicSamlCertificateRepository : ISamlCertificateRepository
    {
        private readonly IEnumerable<SamlCertificate> _issuers;

        public BasicSamlCertificateRepository(IEnumerable<SamlCertificate> issuers)
        {
            _issuers = issuers;
        }

        public SamlCertificate Find(string samlIssuer)
        {
            return _issuers.FirstOrDefault(x => x.Issuer.Equals(samlIssuer, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<SamlCertificate> AllKnownCertificates()
        {
            return _issuers;
        }
    }
}