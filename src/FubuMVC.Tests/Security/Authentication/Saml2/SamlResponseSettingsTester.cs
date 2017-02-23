using FubuCore.Reflection;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Saml2;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2
{
    
    public class SamlResponseSettingsTester
    {
        [Fact]
        public void require_certificate_is_true_by_default()
        {
            new AuthenticationSettings().Saml2.RequireCertificate.ShouldBeTrue();
        }

        [Fact]
        public void require_signature_is_true_by_default()
        {
            new AuthenticationSettings().Saml2.RequireCertificate.ShouldBeTrue();
        }

        [Fact]
        public void enforce_response_time_span_is_true_y_default()
        {
            new AuthenticationSettings().Saml2.EnforceConditionalTimeSpan.ShouldBeTrue();
        }


    }
}