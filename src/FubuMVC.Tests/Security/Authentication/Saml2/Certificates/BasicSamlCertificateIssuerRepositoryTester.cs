using FubuMVC.Core.Security.Authentication.Saml2.Certificates;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2.Certificates
{
    
    public class BasicSamlCertificateIssuerRepositoryTester
    {
        [Fact]
        public void find_by_issuer()
        {
            var issuers = new SamlCertificate[]
            {
                new SamlCertificate{Issuer = "foo:bar1"}, 
                new SamlCertificate{Issuer = "foo:bar2"}, 
                new SamlCertificate{Issuer = "foo:bar3"} 
            };

            var repository = new BasicSamlCertificateRepository(issuers);
            repository.Find(issuers[0].Issuer).ShouldBeTheSameAs(issuers[0]);
            repository.Find(issuers[1].Issuer).ShouldBeTheSameAs(issuers[1]);
            repository.Find(issuers[2].Issuer).ShouldBeTheSameAs(issuers[2]);
        }

        [Fact]
        public void find_all_known()
        {
            var issuers = new SamlCertificate[]
            {
                new SamlCertificate{Issuer = "foo:bar1"}, 
                new SamlCertificate{Issuer = "foo:bar2"}, 
                new SamlCertificate{Issuer = "foo:bar3"} 
            };

            var repository = new BasicSamlCertificateRepository(issuers);

            repository.AllKnownCertificates().ShouldHaveTheSameElementsAs(issuers);
        }
    }
}