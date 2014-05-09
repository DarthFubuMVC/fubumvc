using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Security;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.Security
{
    [TestFixture]
    public class AuthorizationByServiceTester
    {
        [Test]
        public void delegates_by_auth_rights_func()
        {
            var policy = new AuthorizationByService<AuthCheckingService>(x => x.Rights);

            var context = new MockedFubuRequestContext();
            context.Services.As<InMemoryServiceLocator>().Add(new AuthCheckingService
            {
                Rights = AuthorizationRight.Allow
            });

            policy.RightsFor(context).ShouldEqual(AuthorizationRight.Allow);

            context.Services.As<InMemoryServiceLocator>().Add(new AuthCheckingService
            {
                Rights = AuthorizationRight.Deny
            });

            policy.RightsFor(context).ShouldEqual(AuthorizationRight.Deny);

        }

        [Test]
        public void delegates_by_bool_func()
        {
            var policy = new AuthorizationByService<AuthCheckingService>(x => x.IsOk);

            var context = new MockedFubuRequestContext();
            context.Services.As<InMemoryServiceLocator>().Add(new AuthCheckingService
            {
                IsOk = true
            });

            policy.RightsFor(context).ShouldEqual(AuthorizationRight.Allow);

            context.Services.As<InMemoryServiceLocator>().Add(new AuthCheckingService
            {
                IsOk = false
            });

            policy.RightsFor(context).ShouldEqual(AuthorizationRight.Deny);

        }
    }

    public class AuthCheckingService
    {
        public bool IsOk = true;
        public AuthorizationRight Rights = AuthorizationRight.Allow;
    }
}