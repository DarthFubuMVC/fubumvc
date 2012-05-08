﻿using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security
{
    [TestFixture]
    public class when_executing_authorization_policies : InteractionContext<AuthorizationPolicyExecutor>
    {
        private IAuthorizationPolicy[] policies;
        private AuthorizationRight _answer;

        protected override void beforeEach()
        {
            var request = MockFor<IFubuRequest>();

            policies = Services.CreateMockArrayFor<IAuthorizationPolicy>(3);
            policies[0].Expect(x => x.RightsFor(request)).Return(AuthorizationRight.Allow).Repeat.Once();
            policies[1].Expect(x => x.RightsFor(request)).Return(AuthorizationRight.None).Repeat.Once();
            policies[2].Expect(x => x.RightsFor(request)).Return(AuthorizationRight.None).Repeat.Once();
            _answer = ClassUnderTest.IsAuthorized(request, policies);
        }

        [Test]
        public void should_query_all_authorization_policies_once()
        {
            policies[0].VerifyAllExpectations();
            policies[1].VerifyAllExpectations();
            policies[2].VerifyAllExpectations();
        }

        [Test]
        public void should_return_the_combined_result_of_all_policies()
        {
            _answer.ShouldEqual(AuthorizationRight.Allow);
        }
    }


    [TestFixture]
    public class when_executing_recording_authorization_policies : InteractionContext<RecordingAuthorizationPolicyExecutor>
    {
        private IAuthorizationPolicy[] policies;
        private AuthorizationRight _answer;

        protected override void beforeEach()
        {
            var request = MockFor<IFubuRequest>();

            policies = Services.CreateMockArrayFor<IAuthorizationPolicy>(3);
            policies[0].Expect(x => x.RightsFor(request)).Return(AuthorizationRight.Allow).Repeat.Once();
            policies[1].Expect(x => x.RightsFor(request)).Return(AuthorizationRight.None).Repeat.Once();
            policies[2].Expect(x => x.RightsFor(request)).Return(AuthorizationRight.None).Repeat.Once();
            _answer = ClassUnderTest.IsAuthorized(request, policies);
        }

        [Test]
        public void should_query_all_authorization_policies_once()
        {
            policies[0].VerifyAllExpectations();
            policies[1].VerifyAllExpectations();
            policies[2].VerifyAllExpectations();
        }

        [Test]
        public void should_return_the_combined_result_of_all_policies()
        {
            _answer.ShouldEqual(AuthorizationRight.Allow);
        }

        [Test]
        public void should_add_debug_report_details()
        {
            MockFor<IDebugReport>().AssertWasCalled(a => a.AddDetails(null), x => x.IgnoreArguments());
        }
    }

    [TestFixture]
    public class when_executing_recording_authorization_with_no_policies : InteractionContext<RecordingAuthorizationPolicyExecutor>
    {
        private AuthorizationRight _answer;

        protected override void beforeEach()
        {
            var request = MockFor<IFubuRequest>();

            var policies = Services.CreateMockArrayFor<IAuthorizationPolicy>(0);
            _answer = ClassUnderTest.IsAuthorized(request, policies);
        }

        [Test]
        public void should_return_the_combined_result_of_all_policies()
        {
            _answer.ShouldEqual(AuthorizationRight.None);
        }

        [Test]
        public void should_add_debug_report_details()
        {
            MockFor<IDebugReport>().AssertWasNotCalled(a => a.AddDetails(null), x => x.IgnoreArguments());
        }

    }

}