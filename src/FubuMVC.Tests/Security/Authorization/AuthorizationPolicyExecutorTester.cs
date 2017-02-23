using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.Authorization
{
    
    public class when_executing_authorization_policies : InteractionContext<AuthorizationPolicyExecutor>
    {
        private IAuthorizationPolicy[] policies;
        private AuthorizationRight _answer;

        protected override void beforeEach()
        {
            var request = MockFor<IFubuRequestContext>();

            RecordLogging();

            policies = Services.CreateMockArrayFor<IAuthorizationPolicy>(3);
            policies[0].Expect(x => x.RightsFor(request)).Return(AuthorizationRight.Allow).Repeat.Once();
            policies[1].Expect(x => x.RightsFor(request)).Return(AuthorizationRight.None).Repeat.Once();
            policies[2].Expect(x => x.RightsFor(request)).Return(AuthorizationRight.None).Repeat.Once();
            _answer = ClassUnderTest.IsAuthorized(request, policies);
        }

        [Fact]
        public void should_log_the_combined_result()
        {
           RecordedLog().DebugMessages.OfType<AuthorizationResult>().Single().Rights.ShouldBe(_answer);
        }

        [Fact]
        public void should_log_the_result_of_each_policy()
        {
            var results = RecordedLog().DebugMessages.OfType<AuthorizationPolicyResult>();
            results.Select(x => x.Rights.Name).ShouldHaveTheSameElementsAs("Allow", "None", "None");
        }

        [Fact]
        public void should_query_all_authorization_policies_once()
        {
            policies[0].VerifyAllExpectations();
            policies[1].VerifyAllExpectations();
            policies[2].VerifyAllExpectations();
        }

        [Fact]
        public void should_return_the_combined_result_of_all_policies()
        {
            _answer.ShouldBe(AuthorizationRight.Allow);
        }
    }



}