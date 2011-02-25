using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Validation.Registration;

namespace FubuMVC.Validation
{
    public class ConfigureValidationExpression
    {
        private readonly ListDependency _policies = new ListDependency(typeof (IEnumerable<IValidationFailurePolicy>));
        private readonly ValidationCallMatcher _callMatcher = new ValidationCallMatcher();

        public ValidationFailureExpression Failures { get { return new ValidationFailureExpression(_policies); } }
        public ValidationCandidateExpression Validation { get { return new ValidationCandidateExpression(_callMatcher); } }

        public void ConfigureRegistry(FubuRegistry registry)
        {
            registry.Services(x =>
                                  {
                                      var handler = x.SetServiceIfNone(typeof (IValidationFailureHandler), typeof (ValidationFailureHandler));
                                      handler.Dependencies.Add(_policies);
                                  });

            registry
                .ApplyConvention(new ValidationConvention(_callMatcher.CallFilters.Matches));
        }
    }
}