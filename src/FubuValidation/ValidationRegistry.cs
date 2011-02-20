using System;
using System.Collections.Generic;
using FubuCore;
using FubuValidation.Registration;
using FubuValidation.Registration.DSL;
using FubuValidation.Registration.Policies;
using FubuValidation.Registration.Sources;

namespace FubuValidation
{
    public class ValidationRegistry
    {
        private readonly List<IValidationSource> _sources = new List<IValidationSource>();
        private readonly List<IValidationPolicy> _policies = new List<IValidationPolicy>();

        public SourcesExpression Sources { get { return new SourcesExpression(_sources); } }
        public PoliciesExpression Policies { get { return new PoliciesExpression(_policies); } }
        public RulesExpression Rules { get { return new RulesExpression(_policies); } }

        public ValidationRegistry()
        {
            setupDefaults();
        }

        public ValidationRegistry(Action<ValidationRegistry> configure)
            : this()
        {
            configure(this);
        }

        private void setupDefaults()
        {
            Policies
                .ApplyPolicy<ValidationAttributePolicy>()
                .ApplyPolicy<EnumerableValidationPolicy>()
                .ApplyPolicy<ContinuationValidationPolicy>();

            Sources
                .AddSource(new ValidationPolicySource(_policies));
        }

        public IValidationQuery BuildQuery()
        {
            return new ValidationQuery(new TypeResolver(), _sources);
        }
    }
}