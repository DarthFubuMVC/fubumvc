using FubuMVC.Core.Security.Authentication.Windows;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Windows
{
    [TestFixture]
    public class WindowsSignInRequestTester
    {
        private WindowsSignInRequest theRequest;

        [SetUp]
        public void SetUp()
        {
            theRequest = new WindowsSignInRequest();
        }

        [Test]
        public void url_defaults_to_root()
        {
            theRequest.Url.ShouldBe("~/");

        }

        [Test]
        public void uses_specified_url()
        {
            theRequest.Url = "123";
            theRequest.Url.ShouldBe("123");
        }
    }
}