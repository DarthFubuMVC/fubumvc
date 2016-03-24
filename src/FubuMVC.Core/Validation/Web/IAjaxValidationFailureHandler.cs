using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Validation.Web
{
    public interface IAjaxValidationFailureHandler
    {
        void Handle(Notification notification);
    }

    public class AjaxValidationFailureHandler : IAjaxValidationFailureHandler
    {
        private readonly IAjaxContinuationResolver _resolver;
        private readonly IOutputWriter _output;
        private readonly IFubuRequest _request;
        private readonly ValidationSettings _settings;

        public AjaxValidationFailureHandler(IAjaxContinuationResolver resolver, IOutputWriter output, IFubuRequest request, ValidationSettings settings)
        {
            _resolver = resolver;
            _output = output;
            _request = request;
            _settings = settings;
        }

        public void Handle(Notification notification)
        {
            var continuation = _resolver.Resolve(notification);
            
            _output.WriteResponseCode(_settings.StatusCode);

            _request.Set(continuation);
        }
    }
}