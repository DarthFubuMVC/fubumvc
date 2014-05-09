using FubuMVC.Core;
using FubuMVC.Core.Security;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.Security
{
    [TestFixture]
    public class AuthorizationCheckPolicyTester
    {
        [Test]
        public void delegate_to_the_check()
        {
            FakeCheck.Rights = AuthorizationRight.Allow;
            FakeCheck.Created = 0;

            var context = new FubuRequestContext(new StructureMapServiceLocator(new Container()), null, null,null,null);
            var policy = new AuthorizationCheckPolicy<FakeCheck>();

            policy.RightsFor(context).ShouldEqual(AuthorizationRight.Allow);

            FakeCheck.Rights = AuthorizationRight.Deny;

            policy.RightsFor(context).ShouldEqual(AuthorizationRight.Deny);

            FakeCheck.Created.ShouldEqual(2);
        }
    }

    public class FakeCheck : IAuthorizationCheck
    {
        public static AuthorizationRight Rights = AuthorizationRight.Allow;
        public static int Created;


        public FakeCheck()
        {
            Created++;
        }

        public AuthorizationRight Check()
        {
            return Rights;
        }
    }
}