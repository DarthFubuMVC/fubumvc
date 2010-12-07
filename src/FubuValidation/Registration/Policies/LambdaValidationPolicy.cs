using System;
using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuValidation.Registration.Policies
{
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

        public IEnumerable<IValidationRule> BuildRules(Accessor accessor)
        {
            return new[] { _rule(accessor) };
        }
    }
}