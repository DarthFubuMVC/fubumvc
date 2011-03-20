using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;

namespace FubuValidation.Registration
{
    [Obsolete]
    public class ValidationQuery : IValidationQuery
    {
        private readonly IEnumerable<IValidationSource> _sources;
        private readonly ITypeResolver _typeResolver;

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
            throw new NotImplementedException();
            //foreach (var source in _sources)
            //{
            //    var targetRule = source
            //                        .RulesFor(accessor.OwnerType)
            //                        .FirstOrDefault(rule => typeof (T) == _typeResolver.ResolveType(rule) && rule.AppliesTo(accessor))
            //                        .As<T>();

            //    if(targetRule != null)
            //    {
            //        return targetRule;
            //    }
            //}

            //return null;
        }


        public void ForRule<T>(Accessor accessor, Action<T, Accessor> action) where T : class, IValidationRule
        {
            var rule = GetRule<T>(accessor);
            if (rule != null)
            {
                action(rule, accessor);
            }
        }


        public bool HasRule<T>(Accessor accessor)
            where T : class, IValidationRule
        {
            return GetRule<T>(accessor) != null;
        }
    }
}