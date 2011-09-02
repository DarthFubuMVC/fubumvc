using System;
using FubuCore.Binding;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public abstract class ConditionalBehavior : IActionBehavior
    {
        private readonly Func<bool> _shouldExecute;
        private readonly Action _innerInvoke;
        private readonly Action _partialInvoke;

        protected ConditionalBehavior(Func<bool> shouldExecute) 
        {
            _shouldExecute = shouldExecute;
            _innerInvoke = () => { if (InsideBehavior != null) InsideBehavior.Invoke(); };
            _partialInvoke = () => { if (InsideBehavior != null) InsideBehavior.InvokePartial(); };
        }

        public IActionBehavior InsideBehavior { get; set; }

        public void Invoke()
        {
          if(_shouldExecute())
                _innerInvoke();
          
        }

        public void InvokePartial()
        {
            if (_shouldExecute())
                _partialInvoke();
            Invoke();
        }
    }

    public abstract class ConditionalBehavior<T> : ConditionalBehavior
    {
        protected ConditionalBehavior(T context, Func<T, bool> condition)
            : base(() => condition(context))
        {
        }
    }

    public abstract class IsAjaxRequest : ConditionalBehavior<IRequestData>
    {
        protected IsAjaxRequest(IRequestData context)
                            : base(context, x => x.IsAjaxRequest())
        {
        }
    }
}