using System;
using System.Threading.Tasks;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class AsyncContinueWithBehavior<T> : AsyncContinueWithBehavior where T : class
    {
        public AsyncContinueWithBehavior(IFubuRequest fubuRequest)
            : base(fubuRequest)
        {
        }

        protected override void invoke(Action action)
        {
            var task = FubuRequest.Get<Task<T>>();
            task.ContinueWith(x =>
            {
                if (x.IsFaulted) return;

                FubuRequest.Set(task.Result);
                action();
            }, TaskContinuationOptions.AttachedToParent);
        }
    }

    public class AsyncContinueWithBehavior : WrappingBehavior
    {
        private readonly IFubuRequest _fubuRequest;

        public AsyncContinueWithBehavior(IFubuRequest fubuRequest)
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