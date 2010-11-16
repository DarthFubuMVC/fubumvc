using System.Collections.Generic;

namespace FubuValidation.Registration.DSL
{
    public class PoliciesExpression
    {
        private readonly List<IValidationPolicy> _policies;

        public PoliciesExpression(List<IValidationPolicy> policies)
        {
            _policies = policies;
        }

        public PoliciesExpression ApplyPolicy<TPolicy>()
            where TPolicy : IValidationPolicy, new()
        {
            return ApplyPolicy(new TPolicy());
        }

        public PoliciesExpression ApplyPolicy(IValidationPolicy policy)
        {
            _policies.Fill(policy);
            return this;
        }
    }
}