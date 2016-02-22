using System;
using System.Collections.Generic;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation
{
	// Add a new JS source...
	// Then add the field equality JS rule

    public class ValidationGraph
    {
        private readonly IList<IValidationSource> _sources = new List<IValidationSource>();
        private readonly IFieldValidationQuery _fieldQuery;
        private readonly IFieldRulesRegistry _fieldRegistry;
        private readonly Cache<Type, ValidationPlan> _plans; 

        public ValidationGraph(IFieldRulesRegistry fieldRegistry, IEnumerable<IValidationSource> sources)
        {
            _fieldRegistry = fieldRegistry;
            _fieldQuery = new FieldValidationQuery(fieldRegistry);

            _sources.Fill(new FieldRuleSource(fieldRegistry));
            _sources.Fill(new SelfValidatingClassRuleSource());
            _sources.Fill(sources);

            _plans = new Cache<Type, ValidationPlan>(type => ValidationPlan.For(type, this));
        }

        public IEnumerable<IValidationSource> Sources
        {
            get { return _sources; }
        }

        public IFieldRulesRegistry Fields
        {
            get { return _fieldRegistry; }
        }

        public IFieldValidationQuery Query()
        {
            return _fieldQuery;
        }

        /// <summary>
        /// Convenience method for querying against IFieldValidationRules.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="accessor"></param>
        /// <returns></returns>
        public IEnumerable<IFieldValidationRule> FieldRulesFor(Type type, Accessor accessor)
        {
            return Query().RulesFor(type, accessor);
        }

        public void RegisterSource(IValidationSource source)
        {
            _sources.Fill(source);
        }

        public ValidationPlan PlanFor(Type type)
        {
            return _plans[type];
        }

        public void Import(IValidationRegistration registration)
        {
            registration.Register(this);
        }

        public static ValidationGraph BasicGraph()
        {
            return new ValidationGraph(FieldRulesRegistry.BasicRegistry(), new IValidationSource[0]);
        }

        public static ValidationGraph For(IFieldRulesRegistry registry)
        {
            return new ValidationGraph(registry, new IValidationSource[0]);
        }

        /// <summary>
        /// Mostly used for testing. Sets up a ValidationGraph for a given source.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ValidationGraph For(IValidationSource source)
        {
            return new ValidationGraph(FieldRulesRegistry.BasicRegistry(), new[] { source });
        }
    }
}