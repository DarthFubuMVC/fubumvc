using System;
using FubuCore.Binding;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class ConditionalBehavior : IActionBehavior
    {
        public readonly Func<bool> ShouldExecute;
        private readonly Action _innerInvoke;
        private readonly Action _partialInvoke;

        public ConditionalBehavior(Func<bool> shouldExecute) 
        {
            ShouldExecute = shouldExecute;
            _innerInvoke = () => { if (InsideBehavior != null) InsideBehavior.Invoke(); };
            _partialInvoke = () => { if (InsideBehavior != null) InsideBehavior.InvokePartial(); };
        }


        //TODO: Throw a WTF if there is no inner behavior
        public IActionBehavior InsideBehavior { get; set; }

        public void Invoke()
        {
            if (ShouldExecute())
                _innerInvoke();
          
        }

        public void InvokePartial()
        {
            if (ShouldExecute())
                _partialInvoke();
            Invoke();
        }
    }

    public class ConditionalBehavior<T> : ConditionalBehavior
    {
        public ConditionalBehavior(T context, Func<T, bool> condition)
            : base(() => condition(context))
        {
        }
    }

    public class IsAjaxRequest : ConditionalBehavior<IRequestData>
    {
        public IsAjaxRequest(IRequestData context)
                            : base(context, x => x.IsAjaxRequest())
        {
        }
    }
    public class IsNotAjaxRequest : ConditionalBehavior<IRequestData>
    {
        public IsNotAjaxRequest(IRequestData context)
            : base(context, x => !x.IsAjaxRequest())
        {
        }
    }
}