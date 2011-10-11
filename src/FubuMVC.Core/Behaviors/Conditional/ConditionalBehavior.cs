using System;

namespace FubuMVC.Core.Behaviors.Conditional
{
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
            if (_condition.ShouldExecute())
            {
                _innerBehavior.Invoke();
            }
        }

        public void InvokePartial()
        {
            if (_condition.ShouldExecute())
            {
                _innerBehavior.InvokePartial();
            }
        }
    }
}