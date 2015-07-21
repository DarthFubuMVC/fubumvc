using System;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.ServiceBus.Async
{
    public class CascadingAsyncHandlerInvoker<THandler, TInput, TOutput> : BasicBehavior where TInput : class
    {
        private readonly IFubuRequest _request;
        private readonly THandler _handler;
        private readonly Func<THandler, TInput, Task<TOutput>> _func;
        private readonly IAsyncHandling _asyncHandling;

        public CascadingAsyncHandlerInvoker(IFubuRequest request, THandler handler, Func<THandler, TInput, Task<TOutput>> func, IAsyncHandling asyncHandling)
            : base(PartialBehavior.Executes)
        {
            _request = request;
            _handler = handler;
            _func = func;
            _asyncHandling = asyncHandling;
        }

        protected override DoNext performInvoke()
        {
            var input = _request.Find<TInput>().Single();
            var output = _func(_handler, input);

            _asyncHandling.Push(output);

            return DoNext.Continue;
        }
    }
}