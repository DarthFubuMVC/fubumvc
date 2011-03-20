using System;
using System.Collections.Generic;
using FubuValidation.Registration;
using FubuValidation.Registration.DSL;
using FubuValidation.Registration.Policies;
using FubuValidation.Registration.Sources;

namespace FubuValidation
{
    public class ValidationRegistry
    {
        private readonly List<IValidationPolicy> _policies = new List<IValidationPolicy>();
        private readonly List<IValidationSource> _sources = new List<IValidationSource>();

        public ValidationRegistry()
        {
            setupDefaults();
        }

        public ValidationRegistry(Action<ValidationRegistry> configure)
            : this()
        {
            configure(this);
        }

        public SourcesExpression Sources
        {
            get { return new SourcesExpression(_sources); }
        }

        public PoliciesExpression Policies
        {
            get { return new PoliciesExpression(_policies); }
        }

        public RulesExpression Rules
        {
            get { return new RulesExpression(_policies); }
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

        public IEnumerable<IValidationSource> GetConfiguredSources()
        {
            return _sources.ToArray();
        }
    }
}