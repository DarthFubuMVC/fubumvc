using System;
using System.Security.Cryptography.X509Certificates;
using FubuMVC.Core.Security.Authentication.Saml2.Certificates;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2.Certificates
{
    [TestFixture]
    public class CertificateLoaderTester
    {
        private X509Certificate2 certificate2;

        [SetUp]
        public void SetUp()
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            
            certificate2 = ObjectMother.Certificate2();
            store.Add(certificate2);
        }

        [Test]
        public void can_load()
        {
            var loader = new CertificateLoader();
            loader.Load(certificate2.Thumbprint)
                  .SerialNumber.ShouldBe(certificate2.SerialNumber);
        }

        [Test]
        public void throws_unknown_certificate()
        {
            Exception<UnknownCertificateException>.ShouldBeThrownBy(() => {
                new CertificateLoader().Load(Guid.NewGuid().ToString());
            });
        }
    }
}