using FubuMVC.Core.Security.Authorization;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authorization
{
    [TestFixture]
    public class AlwaysDenyPolicyTester
    {
        [Test]
        public void it_is_just_deny()
        {
            new AlwaysDenyPolicy().RightsFor(null)
                .ShouldBe(AuthorizationRight.Deny);
        }
    }
}