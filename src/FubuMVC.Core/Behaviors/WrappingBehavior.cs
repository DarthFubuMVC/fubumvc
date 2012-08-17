using System;

namespace FubuMVC.Core.Behaviors
{
    public abstract class WrappingBehavior : IActionBehavior
    {
        protected WrappingBehavior(IActionBehavior inner)
        {
            Inner = inner;
        }

        public IActionBehavior Inner { get; private set; }

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