using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using FubuMVC.Core.Security.Authentication.Saml2;
using FubuMVC.Core.Security.Authentication.Saml2.Certificates;
using FubuMVC.Core.Security.Authentication.Saml2.Encryption;
using FubuMVC.Core.Security.Authentication.Saml2.Validation;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2.Encryption
{
    [TestFixture, Explicit("Not sure why it's wonky on the server")]
    public class BigBangRoundTripWritingReadingAndEncryptionIntegratedTester
    {
        private X509Certificate2 cert;
        private SamlResponse samlResponse;
        private SamlCertificate samlCert;
        private SamlResponse readResponse;

        [TestFixtureSetUp]
        public void SetUp()
        {
            samlResponse = ObjectMother.Response();

            samlResponse.ShouldNotBeNull();
            samlResponse.Status.ShouldNotBeNull();

            cert = ObjectMother.Certificate2();
            samlCert = ObjectMother.SamlCertificateMatching(samlResponse.Issuer, new X509CertificateWrapper(cert));

            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            store.Add(cert);

            var certificates = new InMemoryCertificateService(samlCert, cert);

            var xml = new SamlResponseWriter(certificates, new SamlResponseXmlSigner(), new AssertionXmlEncryptor()).Write(samlResponse);

            readResponse = new SamlResponseReader(certificates, new AssertionXmlDecryptor()).Read(xml);
        }

        [Test]
        public void can_round_trip_anything()
        {
            readResponse.ShouldNotBeNull();
        }

        [Test]
        public void the_response_has_a_signature()
        {
            readResponse.Signed.ShouldBe(SignatureStatus.Signed);
        }

        [Test]
        public void writes_the_issuer()
        {
            readResponse.Issuer.ShouldBe(samlResponse.Issuer);
        }

        [Test]
        public void writes_the_status()
        {
            readResponse.Status.ShouldBe(samlResponse.Status);
        }

        [Test]
        public void writes_the_Destination()
        {
            readResponse.Destination.ShouldBe(samlResponse.Destination);
        }

        [Test]
        public void writes_the_Id()
        {
            readResponse.Id.ShouldBe(samlResponse.Id);
        }

        [Test]
        public void writes_the_issue_instant()
        {
            readResponse.IssueInstant.ShouldBe(samlResponse.IssueInstant);
        }

        [Test]
        public void writes_the_subject_name()
        {
            readResponse.Subject.Name.ShouldBe(samlResponse.Subject.Name);
        }

        [Test]
        public void writes_the_subject_format()
        {
            readResponse.Subject.Name.Format
                             .ShouldBe(samlResponse.Subject.Name.Format);
        }

        [Test]
        public void writes_the_subject_confirmation_methods()
        {
            readResponse.Subject.Confirmations.Select(x => x.Method)
                .ShouldHaveTheSameElementsAs(samlResponse.Subject.Confirmations.Select(x => x.Method));

        }

        [Test]
        public void writes_the_subject_confirmation_data()
        {
            var secondConfirmationData = readResponse.Subject.Confirmations.First().ConfirmationData.First();
            var originalConfirmationData = samlResponse.Subject.Confirmations.First().ConfirmationData.First();

            secondConfirmationData.NotOnOrAfter.ShouldBe(originalConfirmationData.NotOnOrAfter);
            secondConfirmationData.Recipient.ShouldBe(originalConfirmationData.Recipient);
        }

        [Test]
        public void writes_the_condition_group_times()
        {
            readResponse.Conditions.NotBefore
                             .ShouldBe(samlResponse.Conditions.NotBefore);

            readResponse.Conditions.NotOnOrAfter
                             .ShouldBe(samlResponse.Conditions.NotOnOrAfter);
        }

        [Test]
        public void writes_the_audiences()
        {
            var secondAudiences = readResponse.Conditions.Conditions.OfType<AudienceRestriction>().Select(x => x.Audiences);
            var originalAudiences =
                samlResponse.Conditions.Conditions.OfType<AudienceRestriction>().Select(x => x.Audiences);


            secondAudiences.ShouldHaveTheSameElementsAs(originalAudiences);
        }

        [Test]
        public void writes_the_authentication_context_basic_properties()
        {
            readResponse.Authentication.Instant.ShouldBe(samlResponse.Authentication.Instant);
            readResponse.Authentication.SessionIndex.ShouldBe(samlResponse.Authentication.SessionIndex);
            readResponse.Authentication.SessionNotOnOrAfter.ShouldBe(samlResponse.Authentication.SessionNotOnOrAfter);
        }

        [Test]
        public void writes_the_authentication_context_declaration_reference()
        {
            readResponse.Authentication.DeclarationReference
                             .ShouldBe(samlResponse.Authentication.DeclarationReference);
        }

        [Test]
        public void writes_the_attributes()
        {
            readResponse.Attributes.Get("ClientId")
                             .ShouldBe("000012345");

            readResponse.Attributes.Get("CustomerId")
                             .ShouldBe("001010111");
        }


    }

    public class InMemoryCertificateService : ICertificateService
    {
        private readonly SamlCertificate _certificate;
        private readonly X509Certificate2 _realCertificate;

        public InMemoryCertificateService(SamlCertificate certificate, X509Certificate2 realCertificate)
        {
            _certificate = certificate;
            _realCertificate = realCertificate;
        }

        public SamlValidationKeys Validate(SamlResponse response)
        {
            if (response.Issuer == _certificate.Issuer) return SamlValidationKeys.ValidCertificate;

            return SamlValidationKeys.CannotMatchIssuer;
        }

        public X509Certificate2 LoadCertificate(string issuer)
        {
            return _certificate.Issuer.Equals(issuer, StringComparison.OrdinalIgnoreCase) ? _realCertificate : null;
        }
    }
}