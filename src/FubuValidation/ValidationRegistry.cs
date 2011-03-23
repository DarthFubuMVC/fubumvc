using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuValidation.Fields;

namespace FubuValidation
{
    public interface IValidationRegistration
    {
        void RegisterFieldRules(IFieldRulesRegistry registry);
        IEnumerable<IFieldValidationSource> FieldSources();
    }

    public class ValidationRegistry : IValidationRegistration
    {
        private readonly List<IValidationRegistration> _innerRegistrations = new List<IValidationRegistration>();
        private readonly List<IFieldValidationSource> _sources = new List<IFieldValidationSource>();

        public ValidationRegistry()
        {
        }

        public ValidationRegistry(Action<ValidationRegistry> configure)
            : this()
        {
            configure(this);
        }

        public LambdaFieldValidationSource Required
        {
            get { return ApplyRule<RequiredFieldRule>(); }
        }

        public LambdaFieldValidationSource Continue
        {
            get { return ApplyRule<ContinuationFieldRule>(); }
        }

        void IValidationRegistration.RegisterFieldRules(IFieldRulesRegistry registry)
        {
            _innerRegistrations.Each(i => i.RegisterFieldRules(registry));
        }

        IEnumerable<IFieldValidationSource> IValidationRegistration.FieldSources()
        {
            return _sources.Union(_innerRegistrations.SelectMany(x => x.FieldSources()));
        }

        public void FieldSource<T>() where T : IFieldValidationSource, new()
        {
            FieldSource(new T());
        }

        public void FieldSource(IFieldValidationSource source)
        {
            _sources.Add(source);
        }

        public LambdaFieldValidationSource ApplyRule<T>() where T : IFieldValidationRule, new()
        {
            return ApplyRule(new T());
        }

        private LambdaFieldValidationSource applyPolicy(LambdaFieldValidationSource source)
        {
            _sources.Add(source);
            return source;
        }

        public LambdaFieldValidationSource ApplyRule(IFieldValidationRule rule)
        {
            return applyPolicy(new LambdaFieldValidationSource(rule));
        }

        public LambdaFieldValidationSource ApplyRule(Func<PropertyInfo, IFieldValidationRule> ruleSource)
        {
            return applyPolicy(new LambdaFieldValidationSource(ruleSource));
        }

        public void ForClass<T>(Action<ClassValidationRules<T>> configuration) where T : class
        {
            var rules = new ClassValidationRules<T>();
            configuration(rules);

            _innerRegistrations.Add(rules);
        }
    }
}