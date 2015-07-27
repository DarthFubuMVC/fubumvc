using System;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.RavenDb.Membership;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Raven.Client;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.RavenDb.Tests.Membership
{
    [TestFixture]
    public class LoginAuditPersistorTester : InteractionContext<LoginAuditPersistor>
    {
        private IDocumentSession theSession;
        private LoginRequest theRequest;
        private IEntityRepository theRepository;

        protected override void beforeEach()
        {
            theSession = MockFor<IDocumentSession>();
            theRepository = MockFor<IEntityRepository>();
            theRequest = new LoginRequest();
        }

        [Test]
        public void does_not_apply_history_if_username_is_empty()
        {
            ClassUnderTest.ApplyHistory(theRequest);
            theSession.AssertWasNotCalled(x => x.Load<LoginFailureHistory>((string) null));
            theRequest.NumberOfTries.ShouldBe(0);
            theRequest.LockedOutUntil.ShouldBeNull();
        }

        [Test]
        public void applies_history_if_username_is_not_empty()
        {
            const string userName = "foo";
            theSession.Stub(x => x.Load<LoginFailureHistory>(userName))
                .Return(new LoginFailureHistory {Attempts = 1, LockedOutTime = DateTime.Now});
            theRequest.UserName = userName;
            ClassUnderTest.ApplyHistory(theRequest);
            theSession.AssertWasCalled(x => x.Load<LoginFailureHistory>("foo"));
            theRequest.NumberOfTries.ShouldBe(1);
            theRequest.LockedOutUntil.HasValue.ShouldBeTrue();
        }

        [Test]
        public void does_not_log_failure_if_username_is_empty()
        {
            var audit = new Audit();
            ClassUnderTest.LogFailure(theRequest, audit);
            theRepository.AssertWasNotCalled(x => x.Update(audit));
            theSession.AssertWasNotCalled(x => x.Load<LoginFailureHistory>((string) null));
            theSession.AssertWasNotCalled(x => x.Store(Arg<LoginFailureHistory>.Is.Anything));
        }

        [Test]
        public void logs_failure_if_username_is_not_empty()
        {
            const string userName = "foo";
            var audit = new Audit();
            theSession.Stub(x => x.Load<LoginFailureHistory>(userName))
                .Return(new LoginFailureHistory {Attempts = 1, LockedOutTime = DateTime.Now});
            theRequest.UserName = userName;
            ClassUnderTest.LogFailure(theRequest, audit);
            theRepository.AssertWasCalled(x => x.Update(audit));
            theSession.AssertWasCalled(x => x.Load<LoginFailureHistory>("foo"));
            theSession.AssertWasCalled(x => x.Store(Arg<LoginFailureHistory>.Is.Anything));
        }
    }
}