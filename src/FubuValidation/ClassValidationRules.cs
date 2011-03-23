using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuValidation.Fields;

namespace FubuValidation
{
    public class ClassValidationRules<T> : IValidationRegistration where T : class
    {
        private readonly IList<RuleRegistrationExpression> _rules = new List<RuleRegistrationExpression>();

        public RuleRegistrationExpression Require(params Expression<Func<T, object>>[] properties)
        {
            var accessors = properties.Select(x => ReflectionExtensions.ToAccessor<T>(x));
            var expression = new RuleRegistrationExpression(a => new RequiredFieldRule(), accessors);

            _rules.Add(expression);

            return expression;
        }

        public FieldValidationExpression Property(Expression<Func<T, object>> property)
        {
            return new FieldValidationExpression(this, property.ToAccessor());
        }

        void IValidationRegistration.RegisterFieldRules(IFieldRulesRegistry registration)
        {
            _rules.Each(r => r.Register(registration));
        }

        IEnumerable<IFieldValidationSource> IValidationRegistration.FieldSources()
        {
            yield break;
        }

        

        public class RuleRegistrationExpression
        {
            private Func<Accessor, IFieldValidationRule> _ruleSource;
            private readonly IEnumerable<Accessor> _accessors;

            public RuleRegistrationExpression(Func<Accessor, IFieldValidationRule> ruleSource, Accessor accessor)
                : this(ruleSource, new Accessor[]{accessor})
            {
            }

            public RuleRegistrationExpression(Func<Accessor, IFieldValidationRule> ruleSource, IEnumerable<Accessor> accessors)
            {
                _ruleSource = ruleSource;
                _accessors = accessors;
            }

            public void If(Func<T, bool> filter)
            {
                var innerSource = _ruleSource;
                _ruleSource = a => new ConditionalFieldRule<T>(filter, innerSource(a));
            }

            internal void Register(IFieldRulesRegistry registration)
            {
                _accessors.Each(a => registration.Register(typeof(T), a, _ruleSource(a)));
            } 
        }

        public class FieldValidationExpression
        {
            private readonly ClassValidationRules<T> _parent;
            private readonly Accessor _accessor;
            private RuleRegistrationExpression _lastRule;

            public FieldValidationExpression(ClassValidationRules<T> parent, Accessor accessor)
            {
                _parent = parent;
                _accessor = accessor;
            }

            public void If(Func<T, bool> filter)
            {
                _lastRule.If(filter);
            }

            private FieldValidationExpression register(IFieldValidationRule rule)
            {
                _lastRule = new RuleRegistrationExpression(a => rule, _accessor);
                _parent._rules.Add(_lastRule);

                return this;
            }

            public FieldValidationExpression MaximumLength(int length)
            {
                return register(new MaximumLengthRule(length));
            }

            public FieldValidationExpression GreaterThanZero()
            {
                return register(new GreaterThanZeroRule());
            }

            public FieldValidationExpression GreaterOrEqualToZero()
            {
                return register(new GreaterOrEqualToZeroRule());
            }

            public FieldValidationExpression Required()
            {
                return register(new RequiredFieldRule());
            }
        }
    }
}