using System;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Runtime;
using FubuValidation;

namespace FubuMVC.Validation
{
    public class FubuContinuationFailurePolicy : IValidationFailurePolicy
    {
        private readonly Func<Type, bool> _predicate;
        private readonly IFubuRequest _request;
        private readonly FubuContinuation _continuation;
        private readonly ContinuationHandler _handler;

        public FubuContinuationFailurePolicy(Func<Type, bool> predicate, IFubuRequest request, 
                                             FubuContinuation continuation, ContinuationHandler handler)
        {
            _predicate = predicate;
            _handler = handler;
            _continuation = continuation;
            _request = request;
        }

        public bool Matches(Type modelType)
        {
            return _predicate(modelType);
        }

        public void Handle(Type modelType, Notification notification)
        {
            _request.Set(_continuation);
            _handler.Invoke();
        }
    }
}