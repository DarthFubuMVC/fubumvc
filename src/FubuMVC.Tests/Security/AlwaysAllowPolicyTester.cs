using FubuMVC.Core.Security;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Security
{
    [TestFixture]
    public class AlwaysAllowPolicyTester
    {
        [Test]
        public void it_is_just_allow()
        {
            new AlwaysAllowPolicy().RightsFor(null).ShouldEqual(AuthorizationRight.Allow);
        }
    }
}