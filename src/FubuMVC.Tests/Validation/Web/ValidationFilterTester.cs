using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Web;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web
{
    
    public class ValidationFilterTester : InteractionContext<ValidationFilter<SampleInputModel>>
    {
        private SampleInputModel theInput;
        private Notification theNotification;

        protected override void beforeEach()
        {
            theInput = new SampleInputModel();
            theNotification = new Notification();

            MockFor<IValidator>().Stub(x => x.Validate(theInput)).Return(theNotification);
        }

        [Fact]
        public void builds_up_the_notification()
        {
            ClassUnderTest.Validate(theInput).ShouldBeTheSameAs(theNotification);
        }

        [Fact]
        public void adds_model_binding_errors()
        {
            ClassUnderTest.Validate(theInput);

            MockFor<IModelBindingErrors>().AssertWasCalled(x => x.AddAnyErrors<SampleInputModel>(theNotification));
        }
    }
}