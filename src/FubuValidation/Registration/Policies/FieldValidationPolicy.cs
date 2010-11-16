using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;

namespace FubuValidation.Registration.Policies
{
    public class FieldValidationPolicy<TStrategy> : IValidationPolicy
        where TStrategy : IFieldValidationStrategy, new()
    {
        private readonly Func<Accessor, bool> _predicate;
        private readonly Action<TStrategy> _configure;

        public FieldValidationPolicy(Func<Accessor, bool> predicate, Action<TStrategy> configure)
        {
            _predicate = predicate;
            _configure = configure;
        }

        public bool Matches(Accessor accessor)
        {
            return _predicate(accessor);
        }

        public IEnumerable<IValidationRule> BuildRules(Accessor accessor)
        {
            var strategy = Activator
                .CreateInstance(typeof(TStrategy))
                .As<TStrategy>();

            _configure(strategy);

            return new[] {new FieldRule(accessor, new TypeResolver(), strategy)};
        }
    }
}