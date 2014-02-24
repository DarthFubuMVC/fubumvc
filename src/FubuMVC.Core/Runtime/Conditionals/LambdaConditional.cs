using System;

namespace FubuMVC.Core.Runtime.Conditionals
{
    public class LambdaConditional : IConditional
    {
        private readonly Func<bool> _condition;

        public LambdaConditional(Func<bool> condition)
        {
            _condition = condition;
        }

        public bool ShouldExecute(IFubuRequestContext context)
        {
            return _condition();
        }
    }

    public class LambdaConditional<T> : IConditional
    {
        private readonly Func<T, bool> _condition;

        public LambdaConditional(Func<T, bool> condition)
        {
            _condition = condition;
        }

        public bool ShouldExecute(IFubuRequestContext context)
        {
            return _condition(context.Services.GetInstance<T>());
        }
    }
}