using System;
using System.Collections.Generic;
using FubuMVC.Core.Security.Authentication.Saml2;
using FubuMVC.Core.Security.Authentication.Saml2.Certificates;
using FubuMVC.Core.Security.Authentication.Saml2.Validation;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2.Certificates    
{
    
    public class CertificateServiceTester : InteractionContext<CertificateService>
    {
        [Fact]
        public void load_certificate_when_it_can_be_found()
        {
            var issuer = new SamlCertificate
            {
                Thumbprint = Guid.NewGuid().ToString(),
                Issuer = "foo:bar"
            };

            var cert = ObjectMother.Certificate2();

            MockFor<ICertificateLoader>().Stub(x => x.Load(issuer.Thumbprint))
                                         .Return(cert);

            MockFor<ISamlCertificateRepository>().Stub(x => x.Find(issuer.Issuer))
                                                       .Return(issuer);

            ClassUnderTest.LoadCertificate(issuer.Issuer)
                          .ShouldBeTheSameAs(cert);
        }

        [Fact]
        public void returns_null_if_no_cert_can_be_found_for_that_issuer()
        {
            var issuer = new SamlCertificate
            {
                Thumbprint = Guid.NewGuid().ToString(),
                Issuer = "foo:bar"
            };


            MockFor<ISamlCertificateRepository>().Stub(x => x.Find(issuer.Issuer))
                                                       .Return(issuer);

            ClassUnderTest.LoadCertificate(issuer.Issuer)
                .ShouldBeNull();

        }

        [Fact]
        public void returns_null_if_no_saml_certificate_for_that_issuer()
        {
            ClassUnderTest.LoadCertificate("foo:bar")
                .ShouldBeNull();
        }

        [Fact]
        public void matches_issuer_only_has_to_match_once_single_cert()
        {
            var cert = ObjectMother.Certificate1();
            var response = new SamlResponse
            {
                Issuer = "this:guy",
                Certificates = new ICertificate[] {cert}
            };

            var samlCert = ObjectMother.SamlCertificateMatching(response.Issuer, cert);
            MockFor<ISamlCertificateRepository>().Stub(x => x.Find(response.Issuer)).Return(samlCert);

            ClassUnderTest.MatchesIssuer(response)
                .ShouldBeTrue();
        }

        [Fact]
        public void does_not_match_issuer_if_all_strategies_fail()
        {
            var cert = ObjectMother.Certificate1();
            var response = new SamlResponse
            {
                Issuer = "this:guy",
                Certificates = new ICertificate[] { cert }
            };

            // The mock SamlCertificateRepository will return a null

            ClassUnderTest.MatchesIssuer(response)
                .ShouldBeFalse();
        }

        [Fact]
        public void only_has_to_match_on_one_cert()
        {
            var certs = new ICertificate[]
            {
                new InMemoryCertificate(),
                new InMemoryCertificate(),
                new InMemoryCertificate(),
                new InMemoryCertificate()
            };

            var response = new SamlResponse
            {
                Issuer = "this:guy",
                Certificates = certs
            };

            var samlCert = ObjectMother.SamlCertificateMatching(response.Issuer, certs[3]);
            MockFor<ISamlCertificateRepository>().Stub(x => x.Find(response.Issuer))
                                                 .Return(samlCert);


            ClassUnderTest.MatchesIssuer(response)
                .ShouldBeTrue();
        }

        [Fact]
        public void fails_if_no_certs_match_any_of_the_matchers()
        {
            var certs = new ICertificate[]
            {
                new InMemoryCertificate(),
                new InMemoryCertificate(),
                new InMemoryCertificate(),
                new InMemoryCertificate()
            };

            var response = new SamlResponse
            {
                Issuer = "this:guy",
                Certificates = certs
            };

            // SamlCertificateRepository is returning nulls

            ClassUnderTest.MatchesIssuer(response)
                .ShouldBeFalse();
        }

        [Fact]
        public void returns_CannotMatchIssuer_if_the_cert_does_not_match_the_issuers_we_are_aware_of()
        {
            Services.PartialMockTheClassUnderTest();

            var response = new SamlResponse();

            ClassUnderTest.Expect(x => x.MatchesIssuer(response)).Return(false);

            ClassUnderTest.Validate(response)
                          .ShouldBe(SamlValidationKeys.CannotMatchIssuer);
        }

        [Fact]
        public void return_certificate_is_not_valid_if_all_of_them_fail()
        {
            var response = new SamlResponse();
            var certs = Services.CreateMockArrayFor<ICertificate>(3);
            certs.Each(x => x.Stub(o => o.IsVerified).Return(false));
            response.Certificates = certs;

            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Expect(x => x.MatchesIssuer(response)).Return(true);

            ClassUnderTest.Validate(response)
                          .ShouldBe(SamlValidationKeys.NoValidCertificates);
        }

        [Fact]
        public void return_verified_if_any_certificate_matches_and_is_verified()
        {
            var response = new SamlResponse();
            var certs = Services.CreateMockArrayFor<ICertificate>(3);
            certs[2].Stub(x => x.IsVerified).Return(true);
            response.Certificates = certs;

            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Expect(x => x.MatchesIssuer(response)).Return(true);

            ClassUnderTest.Validate(response)
                          .ShouldBe(SamlValidationKeys.ValidCertificate);
        }
    }
}