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
            var task = _fubuRequest.Get<Task<T>>();
            task.ContinueWith(x =>
            {
                _fubuRequest.Set(x.Result);
                Inner.Invoke();
            }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted);
        }

        public void InvokePartial()
        {
            throw new NotImplementedException("Not implemented");
        }
    }
}