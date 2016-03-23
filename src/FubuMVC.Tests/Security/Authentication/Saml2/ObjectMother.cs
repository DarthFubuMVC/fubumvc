using System;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using FubuCore;
using FubuMVC.Core.Security.Authentication.Saml2;
using FubuMVC.Core.Security.Authentication.Saml2.Certificates;
using FubuMVC.Core.Security.Authentication.Saml2.Xml;

namespace FubuMVC.Tests.Security.Authentication.Saml2
{
    public static class ObjectMother
    {
        public static ICertificate Certificate1()
        {
            var cert = X509Certificate2.CreateFromCertFile("cert1.cer");
            return new X509CertificateWrapper(new X509Certificate2(cert));
        }

        public static X509Certificate2 Certificate2()
        {
            var cert = new X509Certificate2("cert2.pfx", new SecureString(), X509KeyStorageFlags.Exportable);
            return new X509Certificate2(cert);
        }

        public static SamlResponse Response()
        {
            var xml = new FileSystem().ReadStringFromFile("sample.xml");
            return new SamlResponseXmlReader(xml).Read();
        }

        public static SamlCertificate SamlCertificateMatching(string issuer, ICertificate certificate)
        {
            if (certificate == null) throw new ArgumentNullException("certificate");
            return new SamlCertificate
            {
                CertificateIssuer = certificate.Issuer,
                SerialNumber = certificate.SerialNumber,
                Issuer = issuer
            };
        }
    }
}