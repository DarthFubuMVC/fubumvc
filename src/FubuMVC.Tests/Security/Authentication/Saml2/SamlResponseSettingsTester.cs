using FubuCore.Reflection;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Saml2;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2
{
    [TestFixture]
    public class SamlResponseSettingsTester
    {
        [Test]
        public void require_certificate_is_true_by_default()
        {
            new AuthenticationSettings().Saml2.RequireCertificate.ShouldBeTrue();
        }

        [Test]
        public void require_signature_is_true_by_default()
        {
            new AuthenticationSettings().Saml2.RequireCertificate.ShouldBeTrue();
        }

        [Test]
        public void enforce_response_time_span_is_true_y_default()
        {
            new AuthenticationSettings().Saml2.EnforceConditionalTimeSpan.ShouldBeTrue();
        }


    }
}