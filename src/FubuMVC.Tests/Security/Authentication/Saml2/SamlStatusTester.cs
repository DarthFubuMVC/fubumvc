using FubuMVC.Core.Security.Authentication.Saml2;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2
{
    [TestFixture]
    public class SamlStatusTester
    {
        [Test]
        public void can_read_saml_status()
        {
            SamlStatus.Get(SamlStatus.Success.Uri.ToString()).ShouldBeTheSameAs(SamlStatus.Success);
            SamlStatus.Get(SamlStatus.RequesterError.Uri.ToString()).ShouldBeTheSameAs(SamlStatus.RequesterError);
            SamlStatus.Get(SamlStatus.ResponderError.Uri.ToString()).ShouldBeTheSameAs(SamlStatus.ResponderError);
        }
    }
}