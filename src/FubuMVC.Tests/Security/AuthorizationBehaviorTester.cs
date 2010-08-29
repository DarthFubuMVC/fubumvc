using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using NUnit.Framework;
using System.Collections.Generic;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security
{
    [TestFixture]
    public class when_authorization_returns_allow : InteractionContext<AuthorizationBehavior>
    {
        private IAuthorizationPolicy[] policies;

        protected override void beforeEach()
        {
            

            var request = MockFor<IFubuRequest>();

            policies = Services.CreateMockArrayFor<IAuthorizationPolicy>(3);
            policies[0].Stub(x => x.RightsFor(request)).Return(AuthorizationRight.Allow);
            policies[1].Stub(x => x.RightsFor(request)).Return(AuthorizationRight.None);
            policies[2].Stub(x => x.RightsFor(request)).Return(AuthorizationRight.None);

            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();
            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_query_all_authorization_policies()
        {
            policies[0].VerifyAllExpectations();
            policies[1].VerifyAllExpectations();
            policies[2].VerifyAllExpectations();
        }

        [Test]
        public void should_call_into_the_next_behavior()
        {
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }

        [Test]
        public void should_NOT_call_the_authorization_failure_handler()
        {
            MockFor<IAuthorizationFailureHandler>().AssertWasNotCalled(x => x.Handle());
        }
    }

    [TestFixture]
    public class when_authorization_returns_none : InteractionContext<AuthorizationBehavior>
    {
        private IAuthorizationPolicy[] policies;

        protected override void beforeEach()
        {
            var request = MockFor<IFubuRequest>();

            policies = Services.CreateMockArrayFor<IAuthorizationPolicy>(3);
            policies[0].Stub(x => x.RightsFor(request)).Return(AuthorizationRight.None);
            policies[1].Stub(x => x.RightsFor(request)).Return(AuthorizationRight.None);
            policies[2].Stub(x => x.RightsFor(request)).Return(AuthorizationRight.None);


            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();
            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_query_all_authorization_policies()
        {
            policies[0].VerifyAllExpectations();
            policies[1].VerifyAllExpectations();
            policies[2].VerifyAllExpectations();
        }

        [Test]
        public void should_NOT_call_into_the_next_behavior()
        {
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.Invoke());
        }

        [Test]
        public void should_call_the_authorization_failure_handler()
        {
            MockFor<IAuthorizationFailureHandler>().AssertWasCalled(x => x.Handle());
        }
    }


    [TestFixture]
    public class when_authorization_returns_deny : InteractionContext<AuthorizationBehavior>
    {
        private IAuthorizationPolicy[] policies;

        protected override void beforeEach()
        {
            var request = MockFor<IFubuRequest>();

            policies = Services.CreateMockArrayFor<IAuthorizationPolicy>(3);
            policies[0].Stub(x => x.RightsFor(request)).Return(AuthorizationRight.None);
            policies[1].Stub(x => x.RightsFor(request)).Return(AuthorizationRight.Deny);
            policies[2].Stub(x => x.RightsFor(request)).Return(AuthorizationRight.None);


            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();
            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_query_all_authorization_policies()
        {
            policies[0].VerifyAllExpectations();
            policies[1].VerifyAllExpectations();
            policies[2].VerifyAllExpectations();
        }

        [Test]
        public void should_NOT_call_into_the_next_behavior()
        {
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.Invoke());
        }

        [Test]
        public void should_call_the_authorization_failure_handler()
        {
            MockFor<IAuthorizationFailureHandler>().AssertWasCalled(x => x.Handle());
        }
    }
}