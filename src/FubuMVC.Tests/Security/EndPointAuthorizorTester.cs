using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security
{
    [TestFixture]
    public class EndPointAuthorizorTester
    {
        private InMemoryFubuRequest request;

        [SetUp]
        public void SetUp()
        {
            request = new InMemoryFubuRequest();
        }


        private EndPointAuthorizor forAnswers(params AuthorizationRight[] rights)
        {
            var policies = rights.Select(right =>
            {
                var policy = MockRepository.GenerateMock<IAuthorizationPolicy>();
                policy.Stub(x => x.RightsFor(request)).Return(right);

                return policy;
            });

            return new EndPointAuthorizor(policies);
        }

        [Test]
        public void combine_permissions_1()
        {
            forAnswers(AuthorizationRight.Allow, AuthorizationRight.None).IsAuthorized(request)
                .ShouldEqual(AuthorizationRight.Allow);
        }


        [Test]
        public void combine_permissions_2()
        {
            forAnswers(AuthorizationRight.Allow, AuthorizationRight.None, AuthorizationRight.Deny).IsAuthorized(request)
                .ShouldEqual(AuthorizationRight.Deny);
        }


        [Test]
        public void combine_permissions_3()
        {
            forAnswers(AuthorizationRight.None, AuthorizationRight.None).IsAuthorized(request)
                .ShouldEqual(AuthorizationRight.None);
        }
    }
}