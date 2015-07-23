using FubuMVC.Core.Security.Authorization;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authorization
{
    [TestFixture]
    public class AlwaysAllowPolicyTester
    {
        [Test]
        public void it_is_just_allow()
        {
            new AlwaysAllowPolicy().RightsFor(null).ShouldBe(AuthorizationRight.Allow);
        }
    }
}