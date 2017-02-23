using FubuMVC.Core.Security.Authorization;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.Authorization
{
    
    public class AlwaysAllowPolicyTester
    {
        [Fact]
        public void it_is_just_allow()
        {
            new AlwaysAllowPolicy().RightsFor(null).ShouldBe(AuthorizationRight.Allow);
        }
    }
}