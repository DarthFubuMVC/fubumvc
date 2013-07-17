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

        public override void Invoke()
        {
            internalInvoke<Task<T>>(task => FubuRequest.Set(task.Result), x => x.Invoke());
        }
        public override void InvokePartial()
        {
            internalInvoke<Task<T>>(task => FubuRequest.Set(task.Result), x => x.InvokePartial());
        }
    }

    public class AsyncContinueWithBehavior : IActionBehavior
    {
        private readonly IFubuRequest _fubuRequest;
        private readonly IAsyncCoordinator _asyncCoordinator;

        public AsyncContinueWithBehavior(IFubuRequest fubuRequest, IAsyncCoordinator asyncCoordinator)
        {
            _fubuRequest = fubuRequest;
            _asyncCoordinator = asyncCoordinator;
        }

        public IFubuRequest FubuRequest { get { return _fubuRequest; } }
        public IActionBehavior Inner { get; set; }

        protected void internalInvoke<T>(Action<T> action, Action<IActionBehavior> innerAction) where T : Task
        {
            var task = FubuRequest.Get<T>();
            var continuation = task.ContinueWith(x =>
            {
                if (x.IsFaulted)
                    return;

                action(task);
                if (Inner != null)
                {
                    innerAction(Inner);
                }
            });
            _asyncCoordinator.Push(task, continuation);
        }

        public virtual void Invoke()
        {
            internalInvoke<Task>(_ => { }, x => x.Invoke());
        }

        public virtual void InvokePartial()
        {
            internalInvoke<Task>(_ => { }, x => x.InvokePartial());
        }
    }
}