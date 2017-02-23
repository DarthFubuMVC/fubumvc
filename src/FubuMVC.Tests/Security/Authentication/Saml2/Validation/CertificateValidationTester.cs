using System.Linq;
using FubuMVC.Core.Security.Authentication.Saml2;
using FubuMVC.Core.Security.Authentication.Saml2.Certificates;
using FubuMVC.Core.Security.Authentication.Saml2.Validation;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2.Validation
{
    
    public class CertificateValidationTester : InteractionContext<CertificateValidation>
    {
        private SamlResponse response;

        protected override void beforeEach()
        {
            response = new SamlResponse {Certificates = new ICertificate[] {ObjectMother.Certificate1()}};
        }

        private SamlValidationKeys theCertificateValidationReturns
        {
            set
            {
                MockFor<ICertificateService>().Stub(x => x.Validate(response))
                                              .Return(value);
            }
        }

        [Fact]
        public void logs_no_error_if_the_certificate_is_valid()
        {

            theCertificateValidationReturns = SamlValidationKeys.ValidCertificate;

            ClassUnderTest.Validate(response);

            response.Errors.Any().ShouldBeFalse();
        }

        [Fact]
        public void logs_error_if_certificate_does_not_match_issuer()
        {
            theCertificateValidationReturns = SamlValidationKeys.CannotMatchIssuer;
            ClassUnderTest.Validate(response);

            response.Errors.Single().ShouldBe(new SamlError(SamlValidationKeys.CannotMatchIssuer));
        }

        [Fact]
        public void logs_error_if_certificate_is_invalid()
        {
            theCertificateValidationReturns = SamlValidationKeys.NoValidCertificates;
            ClassUnderTest.Validate(response);

            response.Errors.Single().ShouldBe(new SamlError(SamlValidationKeys.NoValidCertificates));
        }
    }
}