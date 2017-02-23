using System.Linq;
using FubuMVC.Core.Security.Authentication.Saml2;
using FubuMVC.Core.Security.Authentication.Saml2.Validation;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2.Validation
{
    
    public class SignatureIsRequiredTester
    {
        [Fact]
        public void no_errors_if_response_is_signed()
        {
            var response = new SamlResponse
            {
                Signed = SignatureStatus.Signed
            };

            new SignatureIsRequired().Validate(response);

            response.Errors.Any().ShouldBeFalse();
        }

        [Fact]
        public void error_if_signature_is_missing()
        {
            var response = new SamlResponse
            {
                Signed = SignatureStatus.NotSigned
            };

            new SignatureIsRequired().Validate(response);

            response.Errors.Single()
                    .ShouldBe(new SamlError(SignatureStatus.NotSigned));
        }

        [Fact]
        public void error_if_signature_is_invalid()
        {
            var response = new SamlResponse
            {
                Signed = SignatureStatus.InvalidSignature
            };

            new SignatureIsRequired().Validate(response);

            response.Errors.Single()
                    .ShouldBe(new SamlError(SignatureStatus.InvalidSignature));
        }
    }
}