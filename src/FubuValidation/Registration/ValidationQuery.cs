using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;

namespace FubuValidation.Registration
{
    public class ValidationQuery : IValidationQuery
    {
        private readonly ITypeResolver _typeResolver;
        private readonly IEnumerable<IValidationSource> _sources;

        public ValidationQuery(ITypeResolver typeResolver, IEnumerable<IValidationSource> sources)
        {
            _typeResolver = typeResolver;
            _sources = sources;
        }

        public IEnumerable<IValidationRule> RulesFor(object target)
        {
            var rules = new List<IValidationRule>();
            _sources.Each(src => rules.AddRange(src.RulesFor(_typeResolver.ResolveType(target))));
            return rules;
        }

        public T GetRule<T>(Accessor accessor) where T : class, IValidationRule
        {
            foreach (var source in _sources)
            {
                var targetRule = source
                                    .RulesFor(accessor.OwnerType)
                                    .FirstOrDefault(rule => typeof (T) == _typeResolver.ResolveType(rule))
                                    .As<T>();

                if(targetRule != null)
                {
                    return targetRule;
                }
            }

            return null;
        }

        public T GetStrategy<T>(Accessor accessor) 
            where T : class, IFieldValidationStrategy
        {
            foreach (var source in _sources)
            {
                var fieldRules = source
                                    .RulesFor(accessor.OwnerType)
                                    .Where(rule => typeof(FieldRule) == rule.GetType())
                                    .Cast<FieldRule>();

                foreach (var fieldRule in fieldRules)
                {
                    if (typeof(IFieldValidationStrategy) == _typeResolver.ResolveType(fieldRule.Strategy))
                    {
                        return fieldRule.Strategy.As<T>();
                    }
                }
                
            }

            return null;
        }

        public void ForRule<T>(Accessor accessor, Action<T, Accessor> action) where T : class, IValidationRule
        {
            var rule = GetRule<T>(accessor);
            if(rule != null)
            {
                action(rule, accessor);
            }
        }

        public void ForStrategy<T>(Accessor accessor, Action<T, Accessor> action) where T : class, IFieldValidationStrategy
        {
            var strategy = GetStrategy<T>(accessor);
            if (strategy != null)
            {
                action(strategy, accessor);
            }
        }

        public bool HasRule<T>(Accessor accessor) where T : IValidationRule
        {
            foreach (var source in _sources)
            {
                if(source.RulesFor(accessor.OwnerType).Any(rule => typeof (T) == _typeResolver.ResolveType(rule)))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasStrategy<T>(Accessor accessor) where T : IFieldValidationStrategy
        {
            foreach (var source in _sources)
            {
                var targetRule = source
                    .RulesFor(accessor.OwnerType)
                    .FirstOrDefault(rule => typeof(FieldRule) == rule.GetType())
                    .As<FieldRule>();

                if (targetRule != null && typeof(IFieldValidationStrategy) == _typeResolver.ResolveType(targetRule.Strategy))
                {
                    return true;
                }
            }

            return false;
        }
    }
}