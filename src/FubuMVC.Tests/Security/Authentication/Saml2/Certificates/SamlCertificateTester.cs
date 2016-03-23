using System;
using FubuMVC.Core.Security.Authentication.Saml2.Certificates;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2.Certificates
{
    [TestFixture]
    public class SamlCertificateTester
    {
        public readonly InMemoryCertificate Certificate = new InMemoryCertificate();

        [Test]
        public void matches_on_both_serial_number_and_certificate_issuer()
        {
            new SamlCertificate
            {
                SerialNumber = Certificate.SerialNumber,
                CertificateIssuer = Certificate.Issuer
            }.Matches(Certificate).ShouldBeTrue();
        }

        [Test]
        public void does_not_match_serial_number_matches_certificate_issuer_does_not()
        {
            new SamlCertificate
            {
                SerialNumber = Guid.NewGuid().ToString(),
                CertificateIssuer = Certificate.Issuer
            }.Matches(Certificate).ShouldBeFalse();
        }

        [Test]
        public void does_not_match_certificate_issuer()
        {
            new SamlCertificate
            {
                SerialNumber = Certificate.SerialNumber,
                CertificateIssuer = Guid.NewGuid().ToString()
            }.Matches(Certificate).ShouldBeFalse();
        }

        [Test]
        public void neither_matches()
        {
            new SamlCertificate
            {
                SerialNumber = Guid.NewGuid().ToString(),
                CertificateIssuer = Guid.NewGuid().ToString()
            }.Matches(Certificate).ShouldBeFalse();
        }

        [Test]
        public void formats_and_load_via_string()
        {
            var cert1 = new SamlCertificate
            {
                Issuer = "foo:bar1",
                SerialNumber = "12345",
                CertificateIssuer = "DN=Foo",
                Thumbprint = "ab cd ef"
            };

            var cert2 = new SamlCertificate(cert1.ToString());

            cert2.ShouldNotBeTheSameAs(cert1);

            cert2.Issuer.ShouldBe(cert1.Issuer);
            cert2.SerialNumber.ShouldBe(cert1.SerialNumber);
            cert2.CertificateIssuer.ShouldBe(cert1.CertificateIssuer);
            cert2.Thumbprint.ShouldBe(cert1.Thumbprint);
        }
    }
}