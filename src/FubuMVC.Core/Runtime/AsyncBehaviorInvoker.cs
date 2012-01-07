using System;
using System.Threading.Tasks;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Runtime
{
    public class AsyncBehaviorInvoker : BehaviorInvoker
    {
        public AsyncBehaviorInvoker(IBehaviorFactory factory, BehaviorChain chain) : base(factory, chain)
        {
        }

        protected override void Invoke(IActionBehavior behavior)
        {
            var task = Task.Factory.StartNew(behavior.Invoke, TaskCreationOptions.AttachedToParent);
            var disposable = behavior as IDisposable;
            if (disposable != null)
            {
                task.ContinueWith(x => disposable.Dispose(), TaskContinuationOptions.AttachedToParent);
            }
        }
    }
}