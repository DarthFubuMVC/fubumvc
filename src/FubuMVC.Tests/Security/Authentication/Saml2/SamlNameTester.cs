using FubuMVC.Core.Security.Authentication.Saml2;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2
{
    
    public class SamlNameTester
    {
        [Fact]
        public void NameID_by_default()
        {
            new SamlName().Type.ShouldBe(SamlNameType.NameID);
        }

        [Fact]
        public void format_is_unspecified_by_default()
        {
            new SamlName().Format.ShouldBe(NameFormat.Unspecified);
        }

        [Fact]
        public void name_format_can_find_itself()
        {
            NameFormat.Get(NameFormat.Persistent.Uri.ToString())
                      .ShouldBeTheSameAs(NameFormat.Persistent);
        }
    }
}