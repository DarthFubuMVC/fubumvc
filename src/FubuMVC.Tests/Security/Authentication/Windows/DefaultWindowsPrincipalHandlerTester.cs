using FubuMVC.Core.Security.Authentication.Windows;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Windows
{
    
    public class DefaultWindowsPrincipalHandlerTester
    {
        [Fact]
        public void authenticated_has_to_return_true()
        {
            new DefaultWindowsPrincipalHandler().Authenticated(null).ShouldBeTrue();
        }
    }
}