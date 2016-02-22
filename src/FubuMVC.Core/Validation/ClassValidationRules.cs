using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation
{
    public interface IFieldConditionalExpression
    {
        void If(IFieldRuleCondition condition);
        void If<T>() where T : IFieldRuleCondition, new();
    }

    public interface IRuleRegistrationExpression : IFieldConditionalExpression
    {
    }

    public interface IFieldValidationExpression : IFieldConditionalExpression
    {
        IFieldValidationExpression Rule(IFieldValidationRule rule);
        IFieldValidationExpression Rule<T>() where T : IFieldValidationRule, new();
    }

    public class ClassValidationRules<T> : IValidationRegistration, IValidationSource where T : class
    {
        private readonly IList<RuleRegistrationExpression> _rules = new List<RuleRegistrationExpression>();
        private readonly IList<IValidationRule> _classRules = new List<IValidationRule>();

        public RuleRegistrationExpression Require(params Expression<Func<T, object>>[] properties)
        {
            var accessors = properties.Select(x => x.ToAccessor());
            var expression = new RuleRegistrationExpression(a => new RequiredFieldRule(), accessors);

            _rules.Add(expression);

            return expression;
        }

        public FieldValidationExpression Property(Expression<Func<T, object>> property)
        {
            var accessor = property.ToAccessor();
            if (accessor.DeclaringType.IsInterface)
            {
                var myProperty = typeof (T).GetProperty(accessor.Name);
                if (myProperty != null)
                {
                    accessor = new SingleProperty(myProperty);
                }
            }

            return new FieldValidationExpression(this, accessor);
        }

        public void Register<TClassRule>() where TClassRule : IValidationRule, new()
        {
            _classRules.Add(new TClassRule());
        }

        public void Register<TClassRule>(TClassRule rule) where TClassRule : IValidationRule
        {
            _classRules.Add(rule);
        }

        void IValidationRegistration.Register(ValidationGraph graph)
        {
            _rules.Each(r => r.Register(graph.Fields));
            graph.RegisterSource(this);
        }

        IEnumerable<IValidationRule> IValidationSource.RulesFor(Type type)
        {
            return type == typeof(T)
                ? _classRules
                : Enumerable.Empty<IValidationRule>();
        }


        public class RuleRegistrationExpression : IRuleRegistrationExpression
        {
            private Func<Accessor, IFieldValidationRule> _ruleSource;
            private readonly IEnumerable<Accessor> _accessors;

            public RuleRegistrationExpression(Func<Accessor, IFieldValidationRule> ruleSource, Accessor accessor)
                : this(ruleSource, new[] { accessor })
            {
            }

            public RuleRegistrationExpression(Func<Accessor, IFieldValidationRule> ruleSource, IEnumerable<Accessor> accessors)
            {
                _ruleSource = ruleSource;
                _accessors = accessors;
            }

            public void If(IFieldRuleCondition condition)
            {
                var innerSource = _ruleSource;
                _ruleSource = a => new ConditionalFieldRule<T>(condition, innerSource(a));
            }

            public void If<TCondition>() where TCondition : IFieldRuleCondition, new()
            {
                If(new TCondition());
            }

            public void If(Func<T, bool> condition)
            {
                If(FieldRuleCondition.For(condition));
            }

            public void If(Func<T, ValidationContext, bool> condition)
            {
                If(FieldRuleCondition.For(condition));
            }

            internal void Register(IFieldRulesRegistry registration)
            {
                _accessors.Each(a => registration.Register(typeof(T), a, _ruleSource(a)));
            } 
        }

        public class FieldValidationExpression : IFieldValidationExpression
        {
            private readonly ClassValidationRules<T> _parent;
            private readonly Accessor _accessor;
            private RuleRegistrationExpression _lastRule;

            public FieldValidationExpression(ClassValidationRules<T> parent, Accessor accessor)
            {
                _parent = parent;
                _accessor = accessor;
            }

            public void If(IFieldRuleCondition condition)
            {
                _lastRule.If(condition);
            }

            public void If<TCondition>() where TCondition : IFieldRuleCondition, new()
            {
                If(new TCondition());
            }

            public void If(Func<T, bool> condition)
            {
                If(FieldRuleCondition.For(condition));
            }

            public void If(Func<T, ValidationContext, bool> condition)
            {
                If(FieldRuleCondition.For(condition));
            }

            public void IfValid()
            {
                If<IsValid>();
            }

            public FieldValidationExpression MaximumLength(int length)
            {
                return register(new MaximumLengthRule(length));
            }

			public FieldValidationExpression MaximumLength(int length, StringToken token)
			{
				return register(new MaximumLengthRule(length, token));
			}

			public FieldValidationExpression MaximumLength(int length, ValidationMode mode)
			{
				return register(new MaximumLengthRule(length) { Mode = mode});
			}

			public FieldValidationExpression MaximumLength(int length, StringToken token, ValidationMode mode)
			{
				return register(new MaximumLengthRule(length, token) { Mode = mode });
			}

            public FieldValidationExpression GreaterThanZero()
            {
                return register(new GreaterThanZeroRule());
            }

			public FieldValidationExpression GreaterThanZero(StringToken token)
			{
				return register(new GreaterThanZeroRule(token));
			}

			public FieldValidationExpression GreaterThanZero(ValidationMode mode)
			{
				return register(new GreaterThanZeroRule { Mode = mode });
			}

			public FieldValidationExpression GreaterThanZero(StringToken token, ValidationMode mode)
			{
				return register(new GreaterThanZeroRule(token) { Mode = mode });
			}

            public FieldValidationExpression GreaterOrEqualToZero()
            {
                return register(new GreaterOrEqualToZeroRule());
            }

			public FieldValidationExpression GreaterOrEqualToZero(StringToken token)
			{
				return register(new GreaterOrEqualToZeroRule(token));
			}

			public FieldValidationExpression GreaterOrEqualToZero(ValidationMode mode)
			{
				return register(new GreaterOrEqualToZeroRule { Mode = mode});
			}

			public FieldValidationExpression GreaterOrEqualToZero(StringToken token, ValidationMode mode)
			{
				return register(new GreaterOrEqualToZeroRule(token) { Mode = mode });
			}

            public FieldValidationExpression Required()
            {
                return register(new RequiredFieldRule());
            }

			public FieldValidationExpression Required(StringToken token)
			{
				return register(new RequiredFieldRule(token));
			}

			public FieldValidationExpression Required(ValidationMode mode)
			{
				return register(new RequiredFieldRule { Mode = mode});
			}
			
			public FieldValidationExpression Required(StringToken token, ValidationMode mode)
			{
				return register(new RequiredFieldRule(token) { Mode = mode});
			}

            public FieldValidationExpression Email()
            {
                return register(new EmailFieldRule());
            }

			public FieldValidationExpression Email(StringToken token)
			{
				return register(new EmailFieldRule(token));
			}

			public FieldValidationExpression Email(ValidationMode mode)
			{
				return register(new EmailFieldRule { Mode = mode});
			}

			public FieldValidationExpression Email(StringToken token, ValidationMode mode)
			{
				return register(new EmailFieldRule(token) { Mode = mode});
			}

            public FieldValidationExpression MinimumLength(int length)
            {
                return register(new MinimumLengthRule(length));
            }

			public FieldValidationExpression MinimumLength(int length, StringToken token)
			{
				return register(new MinimumLengthRule(length, token));
			}

			public FieldValidationExpression MinimumLength(int length, ValidationMode mode)
			{
				return register(new MinimumLengthRule(length) { Mode = mode});
			}

			public FieldValidationExpression MinimumLength(int length, StringToken token, ValidationMode mode)
			{
				return register(new MinimumLengthRule(length, token) { Mode = mode});
			}

            public FieldValidationExpression MinValue(IComparable bounds)
            {
                return register(new MinValueFieldRule(bounds));
            }

			public FieldValidationExpression MinValue(IComparable bounds, StringToken token)
			{
				return register(new MinValueFieldRule(bounds, token));
			}

			public FieldValidationExpression MinValue(IComparable bounds, ValidationMode mode)
			{
				return register(new MinValueFieldRule(bounds) { Mode = mode });
			}

			public FieldValidationExpression MinValue(IComparable bounds, StringToken token, ValidationMode mode)
			{
				return register(new MinValueFieldRule(bounds, token) { Mode = mode});
			}

            public FieldValidationExpression RangeLength(int min, int max)
            {
                return register(new RangeLengthFieldRule(min, max));
            }

			public FieldValidationExpression RangeLength(int min, int max, StringToken token)
			{
				return register(new RangeLengthFieldRule(min, max, token));
			}

			public FieldValidationExpression RangeLength(int min, int max, ValidationMode mode)
			{
				return register(new RangeLengthFieldRule(min, max) { Mode = mode});
			}

			public FieldValidationExpression RangeLength(int min, int max, StringToken token, ValidationMode mode)
			{
				return register(new RangeLengthFieldRule(min, max, token) { Mode = mode});
			}

            public FieldValidationExpression MaxValue(IComparable bounds)
            {
                return register(new MaxValueFieldRule(bounds));
            }

			public FieldValidationExpression MaxValue(IComparable bounds, StringToken token)
			{
				return register(new MaxValueFieldRule(bounds, token));
			}

			public FieldValidationExpression MaxValue(IComparable bounds, ValidationMode mode)
			{
				return register(new MaxValueFieldRule(bounds) { Mode = mode});
			}

			public FieldValidationExpression MaxValue(IComparable bounds, StringToken token, ValidationMode mode)
			{
				return register(new MaxValueFieldRule(bounds, token) { Mode = mode});
			}

			public FieldValidationExpression RegEx(string expression)
			{
				return register(new RegularExpressionFieldRule(expression));
			}

			public FieldValidationExpression RegEx(string expression, StringToken token)
			{
				return register(new RegularExpressionFieldRule(expression, token));
			}

			public FieldValidationExpression RegEx(string expression, ValidationMode mode)
			{
				return register(new RegularExpressionFieldRule(expression) { Mode = mode});
			}

			public FieldValidationExpression RegEx(string expression, StringToken token, ValidationMode mode)
			{
				return register(new RegularExpressionFieldRule(expression, token) { Mode = mode});
			}

			public FieldEqualityRuleExpression Matches(Expression<Func<T, object>> property)
			{
				var rule = new FieldEqualityRule(_accessor, property.ToAccessor());
				_parent.Register(rule);
				return new FieldEqualityRuleExpression(rule);
			}

            private FieldValidationExpression register(IFieldValidationRule rule)
            {
                _lastRule = new RuleRegistrationExpression(a => rule, _accessor);
                _parent._rules.Add(_lastRule);

                return this;
            }

            public IFieldValidationExpression Rule(IFieldValidationRule rule)
            {
                return register(rule);
            }

            public IFieldValidationExpression Rule<TRule>() where TRule : IFieldValidationRule, new()
            {
                return Rule(new TRule());
            }
        }

		public class FieldEqualityRuleExpression
		{
			private readonly FieldEqualityRule _rule;

			public FieldEqualityRuleExpression(FieldEqualityRule rule)
			{
				_rule = rule;
			}

			public FieldEqualityRuleExpression UseToken(StringToken token)
			{
				_rule.Token = token;
				return this;
			}

			public FieldEqualityRuleExpression ReportErrorsOn(Expression<Func<T, object>> property)
			{
				_rule.ReportMessagesFor(property.ToAccessor());
				return this;
			}
		}
    }
}