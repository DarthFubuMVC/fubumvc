using System;

namespace FubuMVC.Core.Behaviors
{
    public enum PartialBehavior
    {
        Ignored,
        Executes
    }

    public abstract class BasicBehavior : IActionBehavior
    {
        private readonly Action _partialInvoke;

        protected BasicBehavior(PartialBehavior partialBehavior)
        {
            _partialInvoke = partialBehavior == PartialBehavior.Executes
                ? (Action) (Invoke)
                : () => { if (InsideBehavior != null) InsideBehavior.InvokePartial(); };
        }

        public IActionBehavior InsideBehavior { get; set; }

        public void Invoke()
        {
            if (performInvoke() == DoNext.Continue && InsideBehavior != null)
            {
                InsideBehavior.Invoke();
            }

            afterInsideBehavior();
        }

        public void InvokePartial()
        {
            _partialInvoke();
        }

        protected virtual DoNext performInvoke()
        {
            return DoNext.Continue;
        }

        protected virtual void afterInsideBehavior()
        {
        }
    }
}