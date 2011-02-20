using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Runtime;
using FubuValidation;

namespace FubuMVC.Validation
{
    public class ValidationFailureHandler : IValidationFailureHandler
    {
        private readonly IEnumerable<IValidationFailurePolicy> _policies;
        private readonly IFubuRequest _request;

        public ValidationFailureHandler(IEnumerable<IValidationFailurePolicy> policies, IFubuRequest request)
        {
            _policies = policies;
            _request = request;
        }

        public void Handle(Type modelType)
        {
            var notification = _request.Get<Notification>();
            var policy = _policies.FirstOrDefault(p => p.Matches(modelType));
            if(policy == null)
            {
                throw new FubuMVCValidationException(1001, notification, "No validation failure policy found for {0}", modelType.FullName);
            }

            policy.Handle(modelType, notification);
        }
    }
}