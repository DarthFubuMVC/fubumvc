using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace FubuMVC.Core.Security.Authentication.Saml2.Certificates
{
    public class InMemoryCertificateLoader : ICertificateLoader
    {
        private readonly X509Certificate2[] _certificates;

        public InMemoryCertificateLoader(params X509Certificate2[] certificates)
        {
            _certificates = certificates;
        }

        public X509Certificate2 Load(string thumbprint)
        {
            return _certificates.FirstOrDefault(x => x.Thumbprint == thumbprint);
        }
    }
}