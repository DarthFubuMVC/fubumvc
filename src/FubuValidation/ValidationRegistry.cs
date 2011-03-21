using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore.Reflection;
using FubuValidation.Fields;
using System.Linq;

namespace FubuValidation
{
    public interface IValidationRegistration
    {
        void RegisterFieldRules(IFieldRulesRegistration registration);
        IEnumerable<IFieldValidationSource> FieldSources();
    }

    public class ValidationRegistry : IValidationRegistration
    {
        private readonly List<IFieldValidationSource> _sources = new List<IFieldValidationSource>();
        private readonly List<IValidationRegistration> _innerRegistrations = new List<IValidationRegistration>();

        public ValidationRegistry()
        {
        }

        public ValidationRegistry(Action<ValidationRegistry> configure)
            : this()
        {
            configure(this);
        }

        public void FieldSource<T>() where T : IFieldValidationSource, new()
        {
            FieldSource(new T());
        }

        public void FieldSource(IFieldValidationSource source)
        {
            _sources.Add(source);
        }

        public LambdaFieldValidationSource Required
        {
            get { return ApplyRule<RequiredFieldRule>(); }
        }

        public LambdaFieldValidationSource Continue
        {
            get { throw new NotImplementedException(); }
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

        void IValidationRegistration.RegisterFieldRules(IFieldRulesRegistration registration)
        {
            _innerRegistrations.Each(i => i.RegisterFieldRules(registration));
        }

        IEnumerable<IFieldValidationSource> IValidationRegistration.FieldSources()
        {
            return _sources.Union(_innerRegistrations.SelectMany(x => x.FieldSources()));
        }
    }
}