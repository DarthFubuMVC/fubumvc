using System;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.ServiceBus.Async
{
    public class AsyncHandlerInvoker<TController, TInput> : BasicBehavior where TInput : class
    {
        private readonly TController _controller;
        private readonly Func<TController, TInput, Task> _func;
        private readonly IFubuRequest _request;
        private readonly IAsyncHandling _asyncHandling;

        public AsyncHandlerInvoker(IFubuRequest request, IAsyncHandling asyncHandling, TController controller,
            Func<TController, TInput, Task> func)
            : base(PartialBehavior.Executes)
        {
            _request = request;
            _asyncHandling = asyncHandling;
            _controller = controller;
            _func = func;
        }

        protected override DoNext performInvoke()
        {
            var input = _request.Find<TInput>().Single();
            var task = _func(_controller, input);
            _asyncHandling.Push(task);

            return DoNext.Continue;
        }
    }
}