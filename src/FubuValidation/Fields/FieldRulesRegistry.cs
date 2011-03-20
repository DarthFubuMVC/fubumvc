using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;
using FubuCore.Util;

namespace FubuValidation.Fields
{
    // TODO -- needs to be registered as a singleton
    public class FieldRulesRegistry : IFieldRulesRegistry
    {
        private readonly IEnumerable<IFieldValidationSource> _sources;
        private readonly ITypeDescriptorCache _typeDescriptors;

        private readonly Cache<Type, ClassFieldValidationRules> _typeRules =
            new Cache<Type, ClassFieldValidationRules>();

        public FieldRulesRegistry(IEnumerable<IFieldValidationSource> sources, ITypeDescriptorCache typeDescriptors)
        {
            _sources = sources;
            _typeDescriptors = typeDescriptors;
            _typeRules.OnMissing = findRules;
        }

        private ClassFieldValidationRules findRules(Type type)
        {
            var classRules = new ClassFieldValidationRules();

            _typeDescriptors.ForEachProperty(type, property =>
            {
                var accessor = new SingleProperty(property);
                var rules = _sources.SelectMany(x => x.RulesFor(property)).Distinct();
                classRules.AddRules(accessor, rules);
            });

            return classRules;
        }

        public ClassFieldValidationRules RulesFor<T>()
        {
            return _typeRules[typeof (T)];
        }

        public ClassFieldValidationRules RulesFor(Type type)
        {
            return _typeRules[type];
        }


        //public void AddRule(Type type, Accessor accessor, IFieldValidationRule rule)
        //{
        //    throw new NotImplementedException();
        //}

        public bool HasRule<T>(Accessor accessor) where T : IFieldValidationRule
        {
            return RulesFor(accessor.OwnerType).HasRule<T>(accessor);
        }

        public void ForRule<T>(Accessor accessor, Action<T> continuation) where T : IFieldValidationRule
        {
            RulesFor(accessor.OwnerType).ForRule<T>(accessor, continuation);
        }
    }
}