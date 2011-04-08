using System;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Validation.Registration
{
    public class ConfigureModelValidationFailureExpression
    {
        private readonly Func<Type, bool> _predicate;
        private readonly ListDependency _policies;

        public ConfigureModelValidationFailureExpression(Func<Type, bool> predicate, ListDependency policies)
        {
            _predicate = predicate;
            _policies = policies;
        }

        public void RedirectTo<T>()
            where T : class, new()
        {
            RedirectTo(new T());
        }

        public void RedirectTo(object target)
        {
            buildPolicy(FubuContinuation.RedirectTo(target));
        }

        public void TransferTo<T>()
            where T : class, new()
        {
            TransferTo(new T());
        }

        public void TransferTo(object target)
        {
            buildPolicy(FubuContinuation.TransferTo(target));
        }

        private void buildPolicy(FubuContinuation continuation)
        {
            var policy = _policies.AddType(typeof (FubuContinuationFailurePolicy));
            policy.DependencyByValue(typeof (Func<Type, bool>), _predicate);
            policy.DependencyByValue(typeof(FubuContinuation), continuation);
        }
    }
}