using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Validation.Web
{
    public class AjaxValidationBehavior<T> : BasicBehavior where T : class
    {
        private readonly IValidationFilter<T> _filter;
        private readonly IFubuRequest _request;
        private readonly IAjaxValidationFailureHandler _failure;

        public AjaxValidationBehavior(IValidationFilter<T> filter, IFubuRequest request, IAjaxValidationFailureHandler failure)
            : base(PartialBehavior.Ignored)
        {
            _filter = filter;
            _request = request;
            _failure = failure;
        }

        protected override DoNext performInvoke()
        {
            var input = _request.Get<T>();
            var notification = _filter.Validate(input);

            if (notification.IsValid())
            {
                return DoNext.Continue;
            }

            _failure.Handle(notification);
            return DoNext.Stop;
        }
    }
}