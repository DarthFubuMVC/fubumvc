using System.Net;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Web;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Validation.Web
{
    
    public class AjaxValidationFailureHandlerTester : InteractionContext<AjaxValidationFailureHandler>
    {
        private AjaxContinuation theAjaxContinuation;
        private Notification theNotification;
        private ValidationSettings theSettings;

        protected override void beforeEach()
        {
            theNotification = new Notification();
            theAjaxContinuation = new AjaxContinuation();

            theSettings = new ValidationSettings();
            theSettings.FailAjaxRequestsWith(HttpStatusCode.SeeOther);

            Services.Inject(theSettings);

            MockFor<IAjaxContinuationResolver>().Stub(x => x.Resolve(theNotification)).Return(theAjaxContinuation);
            ClassUnderTest.Handle(theNotification);
        }

        [Fact]
        public void writes_bad_request_status_code()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.WriteResponseCode(theSettings.StatusCode));
        }

        [Fact]
        public void sets_the_continuation_in_the_request()
        {
            MockFor<IFubuRequest>().AssertWasCalled(x => x.Set(theAjaxContinuation));
        }
    }
}