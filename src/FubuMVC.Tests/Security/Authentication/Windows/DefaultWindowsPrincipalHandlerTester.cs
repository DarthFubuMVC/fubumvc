using FubuMVC.Core.Security.Authentication.Windows;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Windows
{
    [TestFixture]
    public class DefaultWindowsPrincipalHandlerTester
    {
        [Test]
        public void authenticated_has_to_return_true()
        {
            new DefaultWindowsPrincipalHandler().Authenticated(null).ShouldBeTrue();
        }
    }
}