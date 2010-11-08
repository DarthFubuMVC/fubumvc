using System;
using System.Collections.Generic;
using FubuCore.Reflection;
using FubuCore.Util;

namespace FubuValidation.Sources
{
    public class ConventionalValidationSource : IValidationSource
    {
        private readonly Cache<Type, IEnumerable<IValidationRule>> _rules;

        public ConventionalValidationSource(IConventionalValidationRegistry registry)
        {
            _rules = new Cache<Type, IEnumerable<IValidationRule>> { OnMissing = registry.RulesFor };
        }

        public IEnumerable<IValidationRule> RulesFor(Type type)
        {
            return _rules[type];
        }
    }

    public interface IConventionalValidationRegistry
    {
        IEnumerable<IValidationRule> RulesFor(Type type);
    }

    public class ConventionalValidationRegistry : IConventionalValidationRegistry
    {
        public IEnumerable<IValidationRule> RulesFor(Type type)
        {
            throw new NotImplementedException();
        }
    }

    public interface IValidationPolicy
    {
        bool Matches(Accessor accessor);
        IValidationRule BuildRule(Accessor accessor);
    }

    public class LambdaValidationPolicy : IValidationPolicy
    {
        private readonly Func<Accessor, bool> _predicate;
        private readonly Func<Accessor, IValidationRule> _rule;

        public LambdaValidationPolicy(Func<Accessor, bool> predicate, IValidationRule rule)
            : this(predicate, accessor => rule)
        {
        }

        public LambdaValidationPolicy(Func<Accessor, bool> predicate, Func<Accessor, IValidationRule> rule)
        {
            _predicate = predicate;
            _rule = rule;
        }

        public bool Matches(Accessor accessor)
        {
            return _predicate(accessor);
        }

        public IValidationRule BuildRule(Accessor accessor)
        {
            return _rule(accessor);
        }
    }
}