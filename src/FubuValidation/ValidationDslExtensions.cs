using System;
using System.Linq.Expressions;
using FubuValidation.Registration.DSL;
using FubuValidation.Strategies;

namespace FubuValidation
{
    public static class ValidationDslExtensions
    {
        public static RuleConfigurationExpression Required(this RuleConfigurationExpression expression)
        {
            return expression.ApplyStrategy<RequiredFieldStrategy>();
        }

        public static RuleConfigurationExpression GreaterOrEqualToZero(this RuleConfigurationExpression expression)
        {
            return expression.ApplyStrategy<GreaterOrEqualToZeroFieldStrategy>();
        }

        public static RuleConfigurationExpression GreaterThanZero(this RuleConfigurationExpression expression)
        {
            return expression.ApplyStrategy<GreaterThanZeroFieldStrategy>();
        }

        public static RuleConfigurationExpression MaximumStringLength(this RuleConfigurationExpression expression, int length)
        {
            return expression.ApplyStrategy<MaximumStringLengthFieldStrategy>(strategy => strategy.Length = length);
        }

        public static TypedRuleConfigurationExpression<T> Required<T>(this TypedRuleConfigurationExpression<T> ruleExpression, Expression<Func<T, object>> expression)
            where T : class
        {
            return ruleExpression.ApplyStrategy<RequiredFieldStrategy>(expression);
        }

        public static TypedRuleConfigurationExpression<T> GreaterOrEqualToZero<T>(this TypedRuleConfigurationExpression<T> ruleExpression, Expression<Func<T, object>> expression)
             where T : class
        {
            return ruleExpression.ApplyStrategy<GreaterOrEqualToZeroFieldStrategy>(expression);
        }

        public static TypedRuleConfigurationExpression<T> GreaterThanZero<T>(this TypedRuleConfigurationExpression<T> ruleExpression, Expression<Func<T, object>> expression)
             where T : class
        {
            return ruleExpression.ApplyStrategy<GreaterThanZeroFieldStrategy>(expression);
        }

        public static TypedRuleConfigurationExpression<T> MaximumStringLength<T>(this TypedRuleConfigurationExpression<T> ruleExpression, Expression<Func<T, object>> expression, int length)
             where T : class
        {
            return ruleExpression.ApplyStrategy<MaximumStringLengthFieldStrategy>(expression, strategy => strategy.Length = length);
        }
    }
}