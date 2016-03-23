using FubuMVC.Core.Security.Authentication.Saml2;
using FubuMVC.Core.Security.Authentication.Saml2.Validation;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2
{
    [TestFixture]
    public class SamlErrorTester
    {
        [Test]
        public void build_by_string_token()
        {
            var error = new SamlError(SamlValidationKeys.TimeFrameDoesNotMatch);
            error.Key.ShouldBe("TimeFrameDoesNotMatch");
            error.Message.ShouldBe(SamlValidationKeys.TimeFrameDoesNotMatch.ToString());
        }
    }
}