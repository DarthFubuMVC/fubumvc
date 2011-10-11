using System;

namespace FubuMVC.Core.Behaviors.Conditional
{
    public class LambdaConditional : IConditional
    {
        private readonly Func<bool> _condition;

        public LambdaConditional(Func<bool> condition)
        {
            _condition = condition;
        }

        public bool ShouldExecute()
        {
            return _condition();
        }
    }

    public class LambdaConditional<T> : LambdaConditional
    {
        public LambdaConditional(T context, Func<T, bool> condition)
            : base(() => condition(context))
        {
        }
    }
}