using FubuMVC.Core.Security;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Security
{
    [TestFixture]
    public class AlwaysDenyPolicyTester
    {
        [Test]
        public void it_is_just_deny()
        {
            new AlwaysDenyPolicy().RightsFor(null)
                .ShouldEqual(AuthorizationRight.Deny);
        }
    }
}