using System;

namespace FubuMVC.Core.Behaviors
{
    public abstract class WrappingBehavior : IActionBehavior
    {
        public IActionBehavior Inner { get; set; }

        public void Invoke()
        {
            if (Inner != null)
            {
                invoke(() => Inner.Invoke());
            }
        }

        public void InvokePartial()
        {
            if (Inner != null)
            {
                invoke(() => Inner.InvokePartial());
            }
        }

        protected abstract void invoke(Action action);
    }
}