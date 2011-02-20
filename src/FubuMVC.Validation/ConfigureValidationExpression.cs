using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Validation.Registration;

namespace FubuMVC.Validation
{
    public class ConfigureValidationExpression
    {
        private readonly IList<ObjectDef> _policies = new List<ObjectDef>();
        private readonly ValidationCallMatcher _callMatcher = new ValidationCallMatcher();

        public ValidationFailureExpression Failures { get { return new ValidationFailureExpression(_policies); } }
        public ValidationCandidateExpression Validation { get { return new ValidationCandidateExpression(_callMatcher); } }

        public void ConfigureRegistry(FubuRegistry registry)
        {
            registry.Services(x =>
                                  {
                                      var handler = x.SetServiceIfNone(typeof (IValidationFailureHandler<>), typeof (ValidationFailureHandler<>));
                                      var policies = new ListDependency(typeof(IEnumerable<IValidationFailurePolicy>));
                                      _policies.Each(policy => policies.Items.Add(policy));
                                      handler.Dependencies.Add(policies);
                                  });

            registry
                .ApplyConvention(new ValidationConvention(_callMatcher.CallFilters.Matches));
        }
    }
}