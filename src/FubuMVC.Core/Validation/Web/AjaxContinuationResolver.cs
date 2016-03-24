using System.Collections.Generic;
using FubuMVC.Core.Ajax;

namespace FubuMVC.Core.Validation.Web
{
    public interface IAjaxContinuationResolver
    {
        AjaxContinuation Resolve(Notification notification);
    }

    public interface IAjaxContinuationModifier
    {
        void Modify(AjaxContinuation continuation, Notification notification);
    }

     public class AjaxContinuationResolver : IAjaxContinuationResolver
    {
        private readonly IEnumerable<IAjaxContinuationModifier> _modifiers;

        public AjaxContinuationResolver(IEnumerable<IAjaxContinuationModifier> modifiers)
        {
            _modifiers = modifiers;
        }

        public AjaxContinuation Resolve(Notification notification)
        {
            var continuation = AjaxValidation.BuildContinuation(notification);
            _modifiers.Each(x => x.Modify(continuation, notification));

            return continuation;
        }
    }
}