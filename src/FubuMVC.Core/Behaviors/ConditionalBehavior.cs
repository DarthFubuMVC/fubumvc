using System;
using FubuCore.Binding;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public interface IConditionalBehavior : IActionBehavior
    {
    }

    public class ConditionalBehavior : IConditionalBehavior
    {
        private readonly IActionBehavior _innerBehavior;
        private readonly Func<bool> _condition;

        public ConditionalBehavior(IActionBehavior innerBehavior, Func<bool> condition)
        {
            _innerBehavior = innerBehavior;
            _condition = condition;
        }

        public void Invoke()
        {
            if(_condition())
                _innerBehavior.Invoke();
        }

        public void InvokePartial()
        {
            if (_condition())
                _innerBehavior.InvokePartial();
        }
    }

    public class ConditionalBehavior<T> : ConditionalBehavior
    {
        public ConditionalBehavior(IActionBehavior innerBehavior, T context, Func<T, bool> condition)
            : base(innerBehavior,() => condition(context))
        {
        }
    }


    public class IsAjaxRequest : ConditionalBehavior<IRequestData>
    {
        public IsAjaxRequest(IActionBehavior innerBehavior, IRequestData context)
                            : base(innerBehavior, context, x => x.IsAjaxRequest())
        {
        }
    }

    public class IsNotAjaxRequest : ConditionalBehavior<IRequestData>
    {
        public IsNotAjaxRequest(IActionBehavior innerBehavior, IRequestData context)
            : base(innerBehavior, context, x => !x.IsAjaxRequest())
        {
        }
    }
}