using System;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Validation.Registration
{
    public class ValidationFailureExpression
    {
        private readonly ListDependency _policies;

        public ValidationFailureExpression(ListDependency policies)
        {
            _policies = policies;
        }

        public ValidationFailureExpression Add<TPolicy>()
            where TPolicy : IValidationFailurePolicy, new()
        {
            return Add(new TPolicy());
        }

        public ValidationFailureExpression Add(IValidationFailurePolicy policy)
        {
            _policies.AddValue(policy);
            return this;
        }

        public ConfigureModelValidationFailureExpression IfModelType(Func<Type, bool> predicate)
        {
            return new ConfigureModelValidationFailureExpression(predicate, _policies);
        }

        public ConfigureModelValidationFailureExpression IfModelTypeIs<T>()
            where T : class
        {
            return IfModelType(t => t.Equals(typeof (T)));
        }
    }
}