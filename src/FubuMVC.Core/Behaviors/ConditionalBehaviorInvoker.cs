using System;

namespace FubuMVC.Core.Behaviors
{
    public class ConditionalBehaviorInvoker : IActionBehavior
    {
        public ConditionalBehaviorInvoker(IConditionalBehavior behavior)
        {
            _behavior = behavior;
        }

        private readonly IConditionalBehavior _behavior;
        public IActionBehavior InnerBehavior { get; set; }

        public void Invoke()
        {
            _behavior.Invoke();
            if(InnerBehavior != null)
                InnerBehavior.Invoke();
        }

        public void InvokePartial()
        {
            _behavior.InvokePartial();
            if (InnerBehavior != null)
                InnerBehavior.InvokePartial();
        }
    }
}