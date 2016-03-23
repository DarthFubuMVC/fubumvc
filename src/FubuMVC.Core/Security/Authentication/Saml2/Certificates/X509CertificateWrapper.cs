using System.Security.Cryptography.X509Certificates;

namespace FubuMVC.Core.Security.Authentication.Saml2.Certificates
{
    public class X509CertificateWrapper : ICertificate
    {
        private readonly X509Certificate2 _inner;

        public X509CertificateWrapper(X509Certificate2 inner)
        {
            _inner = inner;
            
        }

        public string Issuer
        {
            get { return _inner.Issuer; }
        }

        public string SerialNumber { get { return _inner.SerialNumber; } }

        public bool IsVerified
        {
            get { return _inner.Verify(); }
        }
    }
}