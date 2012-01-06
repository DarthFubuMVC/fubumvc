using System;
using System.Threading.Tasks;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class AsyncContinueWithBehavior<T> : AsyncContinueWithBehavior where T : class
    {
        public AsyncContinueWithBehavior(IFubuRequest fubuRequest) : base(fubuRequest)
        {
        }

        protected override void InnerInvoke(Action<IActionBehavior> behaviorAction)
        {
            var task = FubuRequest.Get<Task<T>>();
            task.ContinueWith(x =>
            {
                FubuRequest.Set(task.Result);
                behaviorAction(InsideBehavior);
            }, TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.AttachedToParent);
        }
    }

    public class AsyncContinueWithBehavior : IActionBehavior
    {
        private readonly IFubuRequest _fubuRequest;

        public AsyncContinueWithBehavior(IFubuRequest fubuRequest)
        {
            _fubuRequest = fubuRequest;
        }

        public IFubuRequest FubuRequest { get { return _fubuRequest; } }
        public IActionBehavior InsideBehavior { get; set; }

        public void Invoke()
        {
            InnerInvoke(x => x.Invoke());
        }

        public void InvokePartial()
        {
            InnerInvoke(x => x.InvokePartial());
        }

        protected virtual void InnerInvoke(Action<IActionBehavior> behaviorAction)
        {
            var task = FubuRequest.Get<Task>();
            task.ContinueWith(x =>
            {
                if (InsideBehavior != null)
                    behaviorAction(InsideBehavior);
            }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted);
        }
    }
}