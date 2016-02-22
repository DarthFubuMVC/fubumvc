using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuCore.Util;

namespace FubuMVC.Core.Validation.Fields
{
    public class FieldRulesRegistry : IFieldRulesRegistry
    {
        private readonly IList<IFieldValidationSource> _sources = new List<IFieldValidationSource>();
        private readonly ITypeDescriptorCache _properties;

        private readonly Cache<Type, ClassFieldValidationRules> _typeRules = new Cache<Type, ClassFieldValidationRules>();

        private FieldRulesRegistry(ITypeDescriptorCache properties)
        {
            _properties = properties;
            _typeRules.OnMissing = findRules;
        }

        public FieldRulesRegistry(IEnumerable<IFieldValidationSource> sources, ITypeDescriptorCache properties)
        {
            _sources.Fill(new AttributeFieldValidationSource());
            _sources.Fill(sources);

            _properties = properties;
            _typeRules.OnMissing = findRules;
        }

        private ClassFieldValidationRules findRules(Type type)
        {
            var classRules = new ClassFieldValidationRules();

            _properties.ForEachProperty(type, property =>
            {
                var accessor = new SingleProperty(property);
                var rules = _sources.SelectMany(x => x.RulesFor(property)).Distinct();
                classRules.AddRules(accessor, rules);
            });

            return classRules;
        }

	    public void FindWith(IFieldValidationSource source)
	    {
		    _sources.Fill(source);
	    }

	    public ClassFieldValidationRules RulesFor<T>()
        {
            return _typeRules[typeof (T)];
        }

        public ClassFieldValidationRules RulesFor(Type type)
        {
            return _typeRules[type];
        }

        public bool HasRule<T>(Accessor accessor) where T : IFieldValidationRule
        {
            return RulesFor(accessor.OwnerType).HasRule<T>(accessor);
        }

        public void ForRule<T>(Accessor accessor, Action<T> continuation) where T : IFieldValidationRule
        {
            RulesFor(accessor.OwnerType).ForRule<T>(accessor, continuation);
        }

        public void Register(Type type, Accessor accessor, IFieldValidationRule rule)
        {
            RulesFor(type).AddRule(accessor, rule);
        }

        /// <summary>
        /// Convenience method to quickly examine all the validation rules
        /// for a given type and accessor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IEnumerable<IFieldValidationRule> RulesFor<T>(Expression<Func<T, object>> expression)
        {
            return RulesFor<T>().RulesFor(expression.ToAccessor());
        }

        public static FieldRulesRegistry BasicRegistry()
        {
            return new FieldRulesRegistry(new IFieldValidationSource[0], new TypeDescriptorCache());
        }

        /// <summary>
        /// Mostly used for testing to avoid the default setup and start with an empty slate.
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static FieldRulesRegistry Explicit(IEnumerable<IFieldValidationSource> sources)
        {
            var registry = new FieldRulesRegistry(new TypeDescriptorCache());
            registry._sources.Fill(sources);

            return registry;
        }
    }
}