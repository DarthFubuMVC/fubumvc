using System;
using System.Threading.Tasks;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class AsyncContinueWithBehavior<T> : AsyncContinueWithBehavior where T : class
    {
        public AsyncContinueWithBehavior(IFubuRequest fubuRequest, IActionBehavior inner) : base(fubuRequest, inner)
        {
        }

        protected override void InnerInvoke(Action<IActionBehavior> behaviorAction)
        {
            var task = FubuRequest.Get<Task<T>>();
            task.ContinueWith(x =>
            {
                FubuRequest.Set(task.Result);
                behaviorAction(Inner);
            }, TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.AttachedToParent);
        }
    }

    public class AsyncContinueWithBehavior : IActionBehavior
    {
        private readonly IFubuRequest _fubuRequest;
        private readonly IActionBehavior _inner;

        public AsyncContinueWithBehavior(IFubuRequest fubuRequest, IActionBehavior inner)
        {
            _fubuRequest = fubuRequest;
            _inner = inner;
        }

        public IFubuRequest FubuRequest { get { return _fubuRequest; } }
        public IActionBehavior Inner { get { return _inner; } }

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
                if (Inner != null)
                    behaviorAction(Inner);
            }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted);
        }
    }
}