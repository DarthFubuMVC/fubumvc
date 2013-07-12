using System;
using System.Threading.Tasks;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class AsyncContinueWithBehavior<T> : AsyncContinueWithBehavior where T : class
    {
        public AsyncContinueWithBehavior(IFubuRequest fubuRequest, IAsyncCoordinator asyncCoordinator)
            : base(fubuRequest, asyncCoordinator)
        {
        }

        protected override void invoke(Action action)
        {
            internalInvoke<Task<T>>(x =>
            {
                FubuRequest.Set(x.Result);
                action();
            });
        }
    }

    public class AsyncContinueWithBehavior : WrappingBehavior
    {
        private readonly IFubuRequest _fubuRequest;
        private readonly IAsyncCoordinator _asyncCoordinator;

        public AsyncContinueWithBehavior(IFubuRequest fubuRequest, IAsyncCoordinator asyncCoordinator)
        {
            _fubuRequest = fubuRequest;
            _asyncCoordinator = asyncCoordinator;
        }

        public IFubuRequest FubuRequest { get { return _fubuRequest; } }

        protected override void invoke(Action action)
        {
            internalInvoke<Task>(x => action());
        }

        protected void internalInvoke<T>(Action<T> action) where T : Task
        {
            var task = FubuRequest.Get<T>();
            var continuation = task.ContinueWith(x =>
            {
                if (x.IsFaulted)
                    return;

                action(task);
            });
            var all = Task.Factory.ContinueWhenAll(new Task[] { task, continuation }, x => { });
            _asyncCoordinator.Push(all);
            _asyncCoordinator.Push(task);
            _asyncCoordinator.Push(continuation);
        }
    }
}