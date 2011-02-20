using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuValidation;

namespace FubuMVC.Validation
{
    public class ValidationBehavior<T> : BasicBehavior
        where T : class
    {
        private readonly IFubuRequest _request;
        private readonly IValidationProvider _provider;
        private readonly IValidationFailureHandler<T> _failureHandler;

        public ValidationBehavior(IFubuRequest request, IValidationProvider provider, IValidationFailureHandler<T> failureHandler) 
            : base(PartialBehavior.Executes)
        {
            _request = request;
            _failureHandler = failureHandler;
            _provider = provider;
        }

        protected override DoNext performInvoke()
        {
            var inputModel = _request.Get<T>();
            var notification = _provider.Validate(inputModel);
            if(notification.IsValid())
            {
                return DoNext.Continue;
            }

            _request.Set(notification);
            _failureHandler.Handle();

            return DoNext.Stop;
        }
    }
}