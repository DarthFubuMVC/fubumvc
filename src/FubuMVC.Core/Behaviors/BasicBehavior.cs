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
        private readonly PartialBehavior _partialBehavior;
        private Action _innerInvoke;

        protected BasicBehavior(PartialBehavior partialBehavior)
        {
            _partialBehavior = partialBehavior;
            _innerInvoke = () => { if (InsideBehavior != null) InsideBehavior.Invoke(); };
        }

        public IActionBehavior InsideBehavior { get; set; }

        public void Invoke()
        {
            if (performInvoke() == DoNext.Continue && InsideBehavior != null)
            {
                _innerInvoke();
            }

            afterInsideBehavior();
        }

        public void InvokePartial()
        {
            if (_partialBehavior == PartialBehavior.Executes)
            {
                _innerInvoke = () => { if (InsideBehavior != null) InsideBehavior.InvokePartial(); };
                Invoke();
            }
            else if (InsideBehavior != null)
            {
                InsideBehavior.InvokePartial();
            }
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