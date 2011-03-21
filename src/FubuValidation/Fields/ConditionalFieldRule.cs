using System;
using FubuCore.Reflection;

namespace FubuValidation.Fields
{
    public class ConditionalFieldRule<T> : IFieldValidationRule where T : class
    {
        private readonly Func<T, bool> _condition;
        private readonly IFieldValidationRule _inner;

        public ConditionalFieldRule(Func<T, bool> condition, IFieldValidationRule inner)
        {
            _condition = condition;
            _inner = inner;
        }

        public void Validate(Accessor accessor, ValidationContext context)
        {
            var target = context.Target as T;
            if (target == null || !_condition(target)) return;

            _inner.Validate(accessor, context);
        }

        public Func<T, bool> Condition
        {
            get { return _condition; }
        }

        public IFieldValidationRule Inner
        {
            get { return _inner; }
        }
    }
}