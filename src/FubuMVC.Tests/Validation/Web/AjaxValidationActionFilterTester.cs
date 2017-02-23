using FubuMVC.Core.Ajax;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Web;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Validation.Web
{
    
    public class AjaxValidationBehaviorTester : InteractionContext<AjaxValidationBehavior<SampleInputModel>>
    {
        private SampleInputModel theInput;
        private Notification theNotification;
        private AjaxContinuation theAjaxContinuation;

        protected override void beforeEach()
        {
            theInput = new SampleInputModel();
            theNotification = new Notification();
            theAjaxContinuation = new AjaxContinuation();

            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();
            MockFor<IFubuRequest>().Stub(x => x.Get<SampleInputModel>()).Return(theInput);
            MockFor<IValidationFilter<SampleInputModel>>().Stub(x => x.Validate(theInput)).Return(theNotification);
            MockFor<IAjaxContinuationResolver>().Stub(x => x.Resolve(theNotification)).Return(theAjaxContinuation);
        }

        [Fact]
        public void continues_to_next_behavior_when_validation_succeeds()
        {
            ClassUnderTest.Invoke();
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }

        [Fact]
        public void writes_the_ajax_continuation_when_validation_fails()
        {
            theNotification.RegisterMessage(StringToken.FromKeyString("Test"));
            ClassUnderTest.Invoke();

            MockFor<IAjaxValidationFailureHandler>().AssertWasCalled(x => x.Handle(theNotification));
        }
    }
}