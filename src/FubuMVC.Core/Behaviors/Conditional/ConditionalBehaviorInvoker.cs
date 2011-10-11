namespace FubuMVC.Core.Behaviors.Conditional
{
    public class ConditionalBehaviorInvoker : IActionBehavior
    {
        private readonly IConditionalBehavior _conditionalBehavior;

        public ConditionalBehaviorInvoker(IConditionalBehavior conditionalBehavior)
        {
            _conditionalBehavior = conditionalBehavior;
        }

        public IActionBehavior InnerBehavior { get; set; }

        public IConditionalBehavior ConditionalBehavior
        {
            get { return _conditionalBehavior; }
        }

        public void Invoke()
        {
            _conditionalBehavior.Invoke();
            if (InnerBehavior != null)
            {
                InnerBehavior.Invoke();
            }
        }

        public void InvokePartial()
        {
            _conditionalBehavior.InvokePartial();
            if (InnerBehavior != null)
            {
                InnerBehavior.InvokePartial();
            }
        }
    }
}