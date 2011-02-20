using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Validation.Registration
{
    public class ValidationFailureExpression
    {
        private readonly IList<ObjectDef> _policies;

        public ValidationFailureExpression(IList<ObjectDef> policies)
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
            _policies.Fill(new ObjectDef(typeof(IValidationFailurePolicy)) { Value = policy });
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