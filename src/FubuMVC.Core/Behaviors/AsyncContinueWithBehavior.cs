using System;
using System.Threading.Tasks;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class AsyncContinueWithBehavior<T> : IActionBehavior where T : class
    {
        private readonly IFubuRequest _fubuRequest;

        public AsyncContinueWithBehavior(IFubuRequest fubuRequest)
        {
            _fubuRequest = fubuRequest;
        }

        public IActionBehavior Inner { get; set; }

        public void Invoke()
        {
            InnerInvoke(x => x.Invoke());
        }

        public void InvokePartial()
        {
            InnerInvoke(x => x.InvokePartial());
        }

        private void InnerInvoke(Action<IActionBehavior> behaviorAction)
        {
            var task = _fubuRequest.Get<Task<T>>();
            task.ContinueWith(x =>
            {
                _fubuRequest.Set(x.Result);
                behaviorAction(Inner);
            }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted);
        }
    }
}