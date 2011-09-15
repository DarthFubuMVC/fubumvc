using System;
using FubuCore.Binding;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public interface IConditionalBehavior : IActionBehavior
    {
    }

    public interface IConditional
    {
        bool ShouldExecute();
    }


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
        public LambdaConditional(T context, Func<T, bool> condition) : base(() => condition(context))
        {
        }
    }

    public class ConditionalBehavior : IConditionalBehavior
    {
        private readonly IActionBehavior _innerBehavior;
        private readonly IConditional _condition;

        public ConditionalBehavior(IActionBehavior innerBehavior, IConditional condition)
        {
            _innerBehavior = innerBehavior;
            _condition = condition;
        }

        public void Invoke()
        {
            if(_condition.ShouldExecute())
                _innerBehavior.Invoke();
        }

        public void InvokePartial()
        {
            if (_condition.ShouldExecute())
                _innerBehavior.InvokePartial();
        }
    }

    public class ConditionalBehavior<T> : ConditionalBehavior
    {
        public ConditionalBehavior(IActionBehavior innerBehavior, IConditional condition) : base(innerBehavior, condition)
        {
        }
    }

    public class IsAjaxRequest : ConditionalBehavior<IRequestData>
    {
        public IsAjaxRequest(IActionBehavior innerBehavior, IRequestData context)
                            : base(innerBehavior, new LambdaConditional<IRequestData>(context, x => x.IsAjaxRequest()))
        {
        }
    }

    public class IsNotAjaxRequest : ConditionalBehavior<IRequestData>
    {
        public IsNotAjaxRequest(IActionBehavior innerBehavior, IRequestData context)
            : base(innerBehavior, new LambdaConditional<IRequestData>(context, x => !x.IsAjaxRequest()))
        {
        }
    }
}