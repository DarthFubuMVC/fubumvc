using FubuLocalization;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using FubuValidation;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Validation.Tests
{
    [TestFixture]
    public class when_validating_an_input_model : InteractionContext<ValidationBehavior<SampleInputModel>>
    {
        protected override void beforeEach()
        {
            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();
        }

        [Test]
        public void should_continue_to_next_behavior_if_validation_succeeds()
        {
            var input = new SampleInputModel();

            MockFor<IFubuRequest>()
                .Expect(request => request.Get<SampleInputModel>())
                .Return(input);

            MockFor<IValidator>()
                .Expect(provider => provider.Validate(input))
                .Return(Notification.Valid());

            ClassUnderTest
                .Invoke();

            MockFor<IActionBehavior>()
                .AssertWasCalled(inner => inner.Invoke());
        }

        [Test]
        public void should_stop_chain_and_put_notification_in_request_and_execute_failure_handler_if_validation_fails()
        {
            var input = new SampleInputModel();
            var notification = new Notification(typeof (SampleInputModel));
            notification.RegisterMessage<SampleInputModel>(m => m.Field, StringToken.FromKeyString("REQUIRED"));

            MockFor<IFubuRequest>()
                .Expect(request => request.Get<SampleInputModel>())
                .Return(input);

            MockFor<IValidator>()
                .Expect(provider => provider.Validate(input))
                .Return(notification);

            MockFor<IFubuRequest>()
                .Expect(request => request.Set(notification));

            MockFor<IValidationFailureHandler>()
                .Expect(handler => handler.Handle(typeof(SampleInputModel)));

            ClassUnderTest
                .Invoke();

            VerifyCallsFor<IFubuRequest>();
            VerifyCallsFor<IValidationFailureHandler>();

            MockFor<IActionBehavior>()
                .AssertWasNotCalled(inner => inner.Invoke());
        }
    }
}