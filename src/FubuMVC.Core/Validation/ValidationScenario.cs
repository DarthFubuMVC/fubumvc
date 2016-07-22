using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation
{
    public class ValidationScenario<T>
    {
        private ValidationScenario(ScenarioDefinition definition)
        {
            var validator = new Validator(new TypeResolver(), definition.Graph(), definition.Services);

            Model = definition.Model;
            Notification = validator.Validate(Model);
        }

        public T Model { get; set; }

        public Notification Notification { get; set; }

        public static ValidationScenario<T> For(Action<ScenarioDefinition> configuration)
        {
            var scenario = new ScenarioDefinition();
            configuration(scenario);

            return new ValidationScenario<T>(scenario);
        }

        public class ScenarioDefinition
        {
            private readonly IList<IValidationRule> _rules = new List<IValidationRule>();
            private readonly IList<IValidationSource> _sources;
            private readonly InMemoryServiceLocator _services = new InMemoryServiceLocator();
            private readonly IList<IFieldValidationRule> _fieldRules = new List<IFieldValidationRule>();
            private readonly IFieldRulesRegistry _fieldRegistry;


            public ScenarioDefinition()
            {
                _sources = new List<IValidationSource> { new ConfiguredValidationSource(_rules) };

                var fieldSource = new PassThruFieldValidationSource(_fieldRules);
                _fieldRegistry = FieldRulesRegistry.Explicit(new IFieldValidationSource[] {fieldSource});
            }

            public T Model { get; set; }
            
            public IServiceLocator Services
            {
                get { return _services; }
            }

            public void Service<TService>(TService service)
            {
                _services.Add(service);
            }

            public void ValidationSource<TSource>(TSource source)
                where TSource : IValidationSource
            {
                _sources.Add(source);
            }

            public void Rule<TRule>(TRule rule)
                where TRule : IValidationRule
            {
                _rules.Add(rule);
            }

            public void FieldRule<TRule>(TRule rule)
                where TRule : IFieldValidationRule
            {
                _fieldRules.Add(rule);
            }

            public ValidationGraph Graph()
            {
                return new ValidationGraph(_fieldRegistry, _sources);
            }
        }

        public class PassThruFieldValidationSource : IFieldValidationSource
        {
            private readonly IEnumerable<IFieldValidationRule> _rules;

            public PassThruFieldValidationSource(IEnumerable<IFieldValidationRule> rules)
            {
                _rules = rules;
            }

            public IEnumerable<IFieldValidationRule> RulesFor(PropertyInfo property)
            {
                return _rules;
            }

            public void AssertIsValid()
            {
            }
        }
    }

    /// <summary>
    /// Used for testing only
    /// </summary>
    public class ConfiguredValidationSource : IValidationSource
    {
        private readonly Func<Type, bool> _predicate; 
        private readonly IEnumerable<IValidationRule> _rules;

        public ConfiguredValidationSource(IEnumerable<IValidationRule> rules)
            : this(type => true, rules)
        {
        }

        public ConfiguredValidationSource(Func<Type, bool> predicate, IEnumerable<IValidationRule> rules)
        {
            _predicate = predicate;
            _rules = rules;
        }

        public IEnumerable<IValidationRule> RulesFor(Type type)
        {
            if(_predicate(type))
            {
                return _rules;
            }

            return new IValidationRule[0];
        }

        public static ConfiguredValidationSource For(params IValidationRule[] rules)
        {
            return new ConfiguredValidationSource(rules);
        }

        public static ConfiguredValidationSource For(Type type, params IValidationRule[] rules)
        {
            return new ConfiguredValidationSource(x => x == type, rules);
        }

        public static ConfiguredValidationSource For(Func<Type, bool> predicate, params IValidationRule[] rules)
        {
            return new ConfiguredValidationSource(predicate, rules);
        }
    }
}