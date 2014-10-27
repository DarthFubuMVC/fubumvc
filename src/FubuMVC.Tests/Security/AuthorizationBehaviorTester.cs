using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Security;
using FubuMVC.Tests.UI.Elements;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security
{
    [TestFixture]
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

        [Test]
        public void should_NOT_call_into_authorization()
        {
            MockFor<IAuthorizationNode>().AssertWasNotCalled(x => x.IsAuthorized(null), _ => _.IgnoreArguments());
        }
    }
}