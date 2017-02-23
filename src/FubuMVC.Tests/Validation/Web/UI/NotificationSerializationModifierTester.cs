using FubuCore;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI.Forms;
using FubuMVC.Core.Urls;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Web;
using FubuMVC.Core.Validation.Web.UI;
using HtmlTags;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
    
    public class NotificationSerializationModifierTester
    {
        private BehaviorGraph theGraph = BehaviorGraph.BuildFrom(x =>
        {
            x.Actions.IncludeType<FormValidationModeEndpoint>();
            x.Features.Validation.Enable(true);
            x.Policies.Local.Add<ValidationPolicy>();
        });

        private NotificationSerializationModifier theModifier;
        private Notification theNotification;
        private IFubuRequest theRequest;

        private AjaxContinuation theContinuation;

        private FormRequest requestFor<T>() where T : class, new()
        {
            var services = new InMemoryServiceLocator();
            services.Add<IChainResolver>(new ChainResolutionCache(theGraph));
            services.Add<IChainUrlResolver>(new ChainUrlResolver(new OwinHttpRequest()));

            theRequest = new InMemoryFubuRequest();
            theNotification = Notification.Valid();
            theRequest.Set(theNotification);

            services.Add(theRequest);

            var request = new FormRequest(new ChainSearch {Type = typeof (T)}, new T());
            request.Attach(services);
            request.ReplaceTag(new FormTag("test"));

            theContinuation = AjaxContinuation.Successful();
            theContinuation.ShouldRefresh = true;

            var resolver = MockRepository.GenerateStub<IAjaxContinuationResolver>();
            resolver.Stub(x => x.Resolve(theNotification)).Return(theContinuation);

            services.Add(resolver);

            return request;
        }

        [Fact]
        public void does_nothing_if_the_notification_is_valid()
        {
            var request = requestFor<LoFiTarget>();
            theModifier.Modify(request);

            request.CurrentTag.Data("validation-results").ShouldBeNull();
        }

        [Fact]
        public void serializes_the_continuation_if_the_notification_is_invalid()
        {
            var request = requestFor<LoFiTarget>();
            theNotification.RegisterMessage(StringToken.FromKeyString("Test", "Test"));
            theModifier.Modify(request);

            request.CurrentTag.Data("validation-results").ShouldBe(theContinuation.ToDictionary());
        }

        [Fact]
        public void writes_the_validation_results_activator_requirement()
        {
            var request = requestFor<LoFiTarget>();
            theNotification.RegisterMessage(StringToken.FromKeyString("Test", "Test"));
            theModifier.Modify(request);
        }
    }
}