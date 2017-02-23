using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.Authorization
{
    
    public class when_authorization_returns_allow : InteractionContext<AuthorizationBehavior>
    {
        protected override void beforeEach()
        {
            var request = MockFor<IFubuRequestContext>();

            MockFor<SecuritySettings>().Reset();

            MockFor<IAuthorizationNode>().Stub(x => x.IsAuthorized(request)).Return(AuthorizationRight.Allow);
            ClassUnderTest.Inner = MockFor<IActionBehavior>();
            ClassUnderTest.Invoke();
        }

        [Fact]
        public void should_call_into_the_next_behavior()
        {
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }

        [Fact]
        public void should_NOT_call_the_authorization_failure_handler()
        {
            MockFor<IAuthorizationFailureHandler>().AssertWasNotCalled(x => x.Handle());
        }
    }

    
    public class when_authorization_is_disabled : InteractionContext<AuthorizationBehavior>
    {
        protected override void beforeEach()
        {
            Services.Inject(new SecuritySettings
            {
                AuthorizationEnabled = false
            });

            ClassUnderTest.Inner = MockFor<IActionBehavior>();
            ClassUnderTest.Invoke();
        }

        [Fact]
        public void should_call_into_the_next_behavior()
        {
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }

        [Fact]
        public void should_NOT_call_the_authorization_failure_handler()
        {
            MockFor<IAuthorizationFailureHandler>().AssertWasNotCalled(x => x.Handle());
        }

        [Fact]
        public void should_NOT_call_into_authorization()
        {
            MockFor<IAuthorizationNode>().AssertWasNotCalled(x => x.IsAuthorized(null), _ => _.IgnoreArguments());
        }
    }
}