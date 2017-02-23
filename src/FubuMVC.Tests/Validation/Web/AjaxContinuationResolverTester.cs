using FubuMVC.Core.Ajax;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Web;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web
{
    
    public class AjaxContinuationResolverTester
    {
        private AjaxContinuationResolver theResolver;
        private RecordingAjaxContinuationModifier theModifier;
        private Notification theNotification;

        public AjaxContinuationResolverTester()
        {
            theModifier = new RecordingAjaxContinuationModifier();
            theResolver = new AjaxContinuationResolver(new[] { theModifier });
            theNotification = new Notification();

            theResolver.Resolve(theNotification);
        }

        [Fact]
        public void modifies_the_continuation()
        {
            theModifier.Continuation.ShouldNotBeNull();
            theModifier.Notification.ShouldBeTheSameAs(theNotification);
        }

        public class RecordingAjaxContinuationModifier : IAjaxContinuationModifier
        {
            public AjaxContinuation Continuation { get; private set; }
            public Notification Notification { get; private set; }

            public void Modify(AjaxContinuation continuation, Notification notification)
            {
                Continuation = continuation;
                Notification = notification;
            }
        }
    }
}