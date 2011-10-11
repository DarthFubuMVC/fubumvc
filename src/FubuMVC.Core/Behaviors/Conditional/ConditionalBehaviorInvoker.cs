namespace FubuMVC.Core.Behaviors.Conditional
{
    public class ConditionalBehaviorInvoker : IActionBehavior
    {
        private readonly IConditionalBehavior _behavior;

        public ConditionalBehaviorInvoker(IConditionalBehavior behavior)
        {
            _behavior = behavior;
        }

        public IActionBehavior InnerBehavior { get; set; }

        public void Invoke()
        {
            _behavior.Invoke();
            if (InnerBehavior != null)
            {
                InnerBehavior.Invoke();
            }
        }

        public void InvokePartial()
        {
            _behavior.InvokePartial();
            if (InnerBehavior != null)
            {
                InnerBehavior.InvokePartial();
            }
        }
    }
}