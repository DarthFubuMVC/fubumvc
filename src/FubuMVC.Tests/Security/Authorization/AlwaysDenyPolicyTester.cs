using FubuMVC.Core.Security.Authorization;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.Authorization
{
    
    public class AlwaysDenyPolicyTester
    {
        [Fact]
        public void it_is_just_deny()
        {
            new AlwaysDenyPolicy().RightsFor(null)
                .ShouldBe(AuthorizationRight.Deny);
        }
    }
}