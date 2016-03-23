using FubuMVC.Core.Security.Authentication.Saml2;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2
{
    [TestFixture]
    public class SamlNameTester
    {
        [Test]
        public void NameID_by_default()
        {
            new SamlName().Type.ShouldBe(SamlNameType.NameID);
        }

        [Test]
        public void format_is_unspecified_by_default()
        {
            new SamlName().Format.ShouldBe(NameFormat.Unspecified);
        }

        [Test]
        public void name_format_can_find_itself()
        {
            NameFormat.Get(NameFormat.Persistent.Uri.ToString())
                      .ShouldBeTheSameAs(NameFormat.Persistent);
        }
    }
}