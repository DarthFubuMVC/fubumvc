using System;
using FubuCore.Reflection;

namespace FubuMVC.Core.Validation.Fields
{
    public interface IFieldRuleCondition
    {
        bool Matches(Accessor accessor, ValidationContext context);
    }

    public static class FieldRuleCondition
    {
        public static LambdaFieldRuleCondition<T> For<T>(Func<T, bool> condition) where T : class
        {
            return new LambdaFieldRuleCondition<T>(condition);
        }

        public static LambdaFieldRuleCondition<T> For<T>(Func<T, ValidationContext, bool> condition) where T : class
        {
            return new LambdaFieldRuleCondition<T>(condition);
        }
    }

    public class LambdaFieldRuleCondition<T> : IFieldRuleCondition where T : class
    {
        private readonly Func<T, ValidationContext, bool> _condition;

        public LambdaFieldRuleCondition(Func<T, bool> condition)
            : this((target, context) => condition(target))
        {
        }

        public LambdaFieldRuleCondition(Func<T, ValidationContext, bool> condition)
        {
            _condition = condition;
        }

        public bool Matches(Accessor accessor, ValidationContext context)
        {
            var target = context.Target as T;
            return target != null && _condition(target, context);
        }
    }
}