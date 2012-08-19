using System;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.Behaviors.Conditional
{
    public class ConditionalBehavior : WrappingBehavior, IConditionalBehavior
    {
        private readonly IConditional _condition;

        public ConditionalBehavior(IActionBehavior innerBehavior, IConditional condition) : base(innerBehavior)
        {
            _condition = condition;
        }


        protected override void invoke(Action action)
        {
            if (_condition.ShouldExecute())
            {
                action();
            }
        }

        public IConditional Condition
        {
            get { return _condition; }
        }
    }
}