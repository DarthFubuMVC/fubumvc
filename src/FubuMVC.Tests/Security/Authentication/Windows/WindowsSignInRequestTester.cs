using FubuMVC.Core.Security.Authentication.Windows;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Windows
{
    
    public class WindowsSignInRequestTester
    {
        private WindowsSignInRequest theRequest = new WindowsSignInRequest();

        [Fact]
        public void url_defaults_to_root()
        {
            theRequest.Url.ShouldBe("~/");

        }

        [Fact]
        public void uses_specified_url()
        {
            theRequest.Url = "123";
            theRequest.Url.ShouldBe("123");
        }
    }
}