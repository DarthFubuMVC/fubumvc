using System;
using System.Collections.Generic;
using FubuCore.Logging;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Auditing;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.Authentication
{
    [TestFixture]
    public class AuthenticationServiceTester : InteractionContext<AuthenticationService>
    {
        private IAuthenticationStrategy[] theStrategies;

        protected override void beforeEach()
        {
            theStrategies = Services.CreateMockArrayFor<IAuthenticationStrategy>(4);
        }

        [Test]
        public void constructor_function_throws_exception_if_there_are_no_strategies()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                new AuthenticationService(new RecordingLogger(), new IAuthenticationStrategy[0], null, null);
            });
        }

        [Test]
        public void stops_if_any_strategy_is_deterministic()
        {
            var result = new AuthResult {Continuation = FubuContinuation.RedirectTo("somewhere"), Success = false};
            theStrategies[0].Stub(x => x.TryToApply()).Return(result);

            ClassUnderTest.TryToApply().ShouldBeTheSameAs(result);
        }

        [Test]
        public void try_to_apply_fails_if_all_fail()
        {
            theStrategies.Each(x => x.Stub(o => o.TryToApply()).Return(AuthResult.Failed()));

            ClassUnderTest.TryToApply().Success.ShouldBeFalse();
        }


        [Test]
        public void try_to_apply_succeeds_if_any_succeeds()
        {
            theStrategies[0].Stub(x => x.TryToApply()).Return(AuthResult.Failed());
            theStrategies[1].Stub(x => x.TryToApply()).Return(AuthResult.Failed());
            theStrategies[2].Stub(x => x.TryToApply()).Return(AuthResult.Failed());
            theStrategies[3].Stub(x => x.TryToApply()).Return(AuthResult.Successful());

            ClassUnderTest.TryToApply().Success.ShouldBeTrue();
        }

        [Test]
        public void authenticate_fails_if_all_fail()
        {
            var request = new LoginRequest();

            theStrategies.Each(x => x.Stub(o => o.Authenticate(request)).Return(false));

            ClassUnderTest.Authenticate(request).ShouldBeFalse();
        }


        [Test]
        public void authenticate_succeeds_if_any_succeeds()
        {
            var request = new LoginRequest();

            theStrategies[3].Stub(x => x.Authenticate(request)).Return(true);

            ClassUnderTest.Authenticate(request).ShouldBeTrue();
        }


        [Test]
        public void should_have_applied_history()
        {
            var request = new LoginRequest();
            ClassUnderTest.Authenticate(request);
            MockFor<ILoginAuditor>().AssertWasCalled(x => x.ApplyHistory(request));
        }

        [Test]
        public void should_audit_the_request()
        {
            var request = new LoginRequest();
            ClassUnderTest.Authenticate(request);
            MockFor<ILoginAuditor>().AssertWasCalled(x => x.Audit(request));
        }

        [Test]
        public void executes_strategies_sent_through_parameter()
        {
            var request = new LoginRequest();
            IAuthenticationStrategy[] paramStrategies = Services.CreateMockArrayFor<IAuthenticationStrategy>(1);
            ClassUnderTest.Authenticate(request, paramStrategies);
            paramStrategies[0].AssertWasCalled(x => x.Authenticate(request));
        }
    }
}