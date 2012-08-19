using System;
using System.Threading.Tasks;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class AsyncContinueWithBehavior<T> : AsyncContinueWithBehavior where T : class
    {
        public AsyncContinueWithBehavior(IFubuRequest fubuRequest, IActionBehavior inner)
            : base(fubuRequest, inner)
        {
        }

        protected override void invoke(Action action)
        {
            var task = FubuRequest.Get<Task<T>>();
            task.ContinueWith(x =>
            {
                FubuRequest.Set(task.Result);
                action();
            }, TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.AttachedToParent);
        }
    }

    public class AsyncContinueWithBehavior : WrappingBehavior
    {
        private readonly IFubuRequest _fubuRequest;

        public AsyncContinueWithBehavior(IFubuRequest fubuRequest, IActionBehavior inner) : base(inner)
        {
            _fubuRequest = fubuRequest;
        }

        public IFubuRequest FubuRequest { get { return _fubuRequest; } }

        protected override void invoke(Action action)
        {
            var task = FubuRequest.Get<Task>();
            task.ContinueWith(x => action(), TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted);
        }
    }
}