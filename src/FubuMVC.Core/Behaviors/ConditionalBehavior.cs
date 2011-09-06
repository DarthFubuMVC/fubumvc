using System;
using FubuCore.Binding;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public interface IConditionalInvoker
    {
        void Invoke(bool condition);
    }
    public class ConditionalInvoker :IConditionalInvoker
    {
        private readonly IActionBehavior _innerBehavior;

        public ConditionalInvoker(IActionBehavior innerBehavior)
        {
            _innerBehavior = innerBehavior;
        }

        public void Invoke(bool condition)
        {
            if(condition)
            _innerBehavior.Invoke();
        }
    }

    public class ConditionalBehavior : IActionBehavior
    {
        public readonly Func<bool> ShouldExecute;
        private readonly Action _innerInvoke;
        private readonly Action _partialInvoke;

        public ConditionalBehavior(IActionBehavior innerBehavior, Func<bool> shouldExecute)
        {
            _insideBehavior = innerBehavior;
            ShouldExecute = shouldExecute;
            _innerInvoke = () => { if (_insideBehavior != null) _insideBehavior.Invoke(); };
            _partialInvoke = () => { if (_insideBehavior != null) _insideBehavior.InvokePartial(); };
        }


        private IActionBehavior _insideBehavior;
       

        public void Invoke()
        {
            if (ShouldExecute())
                _innerInvoke();
          
        }

        public void InvokePartial()
        {
            if (ShouldExecute())
                _partialInvoke();
      
        }
    }

    public class ConditionalBehavior<T> : ConditionalBehavior
    {
        public ConditionalBehavior(IActionBehavior innerBehavior,T context, Func<T, bool> condition)
            : base(innerBehavior,() => condition(context))
        {
        }
    }


    public class IsAjaxRequest : ConditionalBehavior<IRequestData>
    {
        public IsAjaxRequest(IActionBehavior innerBehavior, IRequestData context)
                            : base(innerBehavior, context, x => x.IsAjaxRequest())
        {
        }
    }

    public class IsNotAjaxRequest : ConditionalBehavior<IRequestData>
    {
        public IsNotAjaxRequest(IActionBehavior innerBehavior, IRequestData context)
            : base(innerBehavior, context, x => !x.IsAjaxRequest())
        {
        }
    }
}