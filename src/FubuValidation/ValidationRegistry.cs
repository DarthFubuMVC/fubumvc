using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore.Reflection;
using FubuValidation.Fields;

namespace FubuValidation
{
    public interface IValidationRegistration
    {
        void RegisterFieldRules(IFieldRulesRegistration registration);
        IEnumerable<IFieldValidationSource> FieldSources();
    }

    public class LambdaFieldValidationSource : IFieldValidationSource
    {
        private readonly Func<PropertyInfo, IFieldValidationRule> _ruleSource;

        public LambdaFieldValidationSource(IFieldValidationRule rule)
        {
            _ruleSource = prop => rule;
        }

        public LambdaFieldValidationSource(Func<PropertyInfo, IFieldValidationRule> ruleSource)
        {
            _ruleSource = ruleSource;
        }

        IEnumerable<IFieldValidationRule> IFieldValidationSource.RulesFor(PropertyInfo property)
        {
            throw new NotImplementedException();
        }

        public void If(Func<Accessor, bool> filter)
        {
            throw new NotImplementedException();
        }

        public void IfPropertyType<T>()
        {
            throw new NotImplementedException();
        }

        public void IfPropertyType(Func<Type, bool> filter)
        {
            throw new NotImplementedException();
        }
    }

    public class ValidationRegistry : IValidationRegistration
    {
        private readonly List<IValidationSource> _sources = new List<IValidationSource>();

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
            throw new NotImplementedException();
        }

        public void FieldSource(IFieldValidationSource source)
        {
            throw new NotImplementedException();
        }

        public LambdaFieldValidationSource Required
        {
            get { throw new NotImplementedException(); }
        }

        public LambdaFieldValidationSource Continue
        {
            get { throw new NotImplementedException(); }
        }

        public LambdaFieldValidationSource ApplyRule<T>() where T : IFieldValidationRule, new()
        {
            throw new NotImplementedException();
        }

        public LambdaFieldValidationSource ApplyRule(IFieldValidationRule rule)
        {
            throw new NotImplementedException();
        }

        public LambdaFieldValidationSource ApplyRule(Func<Accessor, IFieldValidationRule> ruleSource)
        {
            throw new NotImplementedException();
        }

        void IValidationRegistration.RegisterFieldRules(IFieldRulesRegistration registration)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IFieldValidationSource> IValidationRegistration.FieldSources()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IValidationSource> GetConfiguredSources()
        {
            return _sources.ToArray();
        }
    }
}