using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Core.Security.Authentication.Saml2.Certificates
{
    public class SamlCertificate
    {
        public string SerialNumber { get; set; }
        public string CertificateIssuer { get; set; }
        public string Issuer { get; set; }
        public string Thumbprint { get; set; }


        public bool Matches(ICertificate certificate)
        {
            return certificate.SerialNumber == SerialNumber &&
                   certificate.Issuer == CertificateIssuer;
        }

        public SamlCertificate()
        {
        }

        public SamlCertificate(string text)
        {
            var parts = text.ToDelimitedArray();

            Issuer = parts[0];
            CertificateIssuer = parts[1];
            SerialNumber = parts[2];
            Thumbprint = parts[3];
        }

        public override string ToString()
        {
            return new string[] {Issuer.ToString(), CertificateIssuer, SerialNumber, Thumbprint}.Join(",");
        }
    }
}