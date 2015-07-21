using System;
using System.Linq;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public class CascadingHandlerInvoker<THandler, TInput, TOutput> : BasicBehavior where TInput : class
    {
        private readonly IFubuRequest _request;
        private readonly THandler _handler;
        private readonly IInvocationContext _messages;
        private readonly Func<THandler, TInput, TOutput> _action;

        public CascadingHandlerInvoker(IFubuRequest request, THandler handler, Func<THandler, TInput, TOutput> action, IInvocationContext messages) : base(PartialBehavior.Executes)
        {
            _request = request;
            _handler = handler;
            _action = action;
            _messages = messages;
        }

        protected override DoNext performInvoke()
        {
            var input = _request.Find<TInput>().Single();
            var output = _action(_handler, input);

            _messages.EnqueueCascading(output);

            return DoNext.Continue;
        }
    }
}