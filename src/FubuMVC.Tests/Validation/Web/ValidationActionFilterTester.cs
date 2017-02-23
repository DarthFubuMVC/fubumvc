using FubuMVC.Core.Continuations;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Web;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Validation.Web
{
    
    public class ValidationActionFilterTester : InteractionContext<ValidationActionFilter<ActionFilterTarget>>
    {
        private ActionFilterTarget theTarget;
        private Notification theNotification;

        protected override void beforeEach()
        {
            theTarget = new ActionFilterTarget();
            theNotification = new Notification();
            MockFor<IValidationFilter<ActionFilterTarget>>().Stub(x => x.Validate(theTarget)).Return(theNotification);
        }

        private FubuContinuation theContinuation { get { return ClassUnderTest.Validate(theTarget); } }

        [Fact]
        public void continues_to_next_behavior_when_validation_succeeds()
        {
            theContinuation.AssertWasContinuedToNextBehavior();
        }

        [Fact]
        public void transfers_to_GET_category_of_input_type_when_validation_fails()
        {
            theNotification.RegisterMessage(StringToken.FromKeyString("Test"));
            theContinuation.AssertWasTransferedTo(theTarget, "GET");
        }

        [Fact]
        public void sets_the_notification_in_the_request()
        {
            theNotification.RegisterMessage(StringToken.FromKeyString("Test"));
            ClassUnderTest.Validate(theTarget);
            MockFor<IFubuRequest>().AssertWasCalled(x => x.Set(theNotification));
        }
    }

    public class ActionFilterTarget
    {
    }
}