using System;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class ZeroInOneOutActionInvoker<TController, TOutput> : BasicBehavior where TOutput : class
    {
        private readonly Func<TController, TOutput> _action;
        private readonly TController _controller;
        private readonly IFubuRequest _request;

        public ZeroInOneOutActionInvoker(IFubuRequest request, TController controller, Func<TController, TOutput> action)
            : base(PartialBehavior.Executes)
        {
            _request = request;
            _controller = controller;
            _action = action;
        }

        protected override DoNext performInvoke()
        {
            TOutput output = _action(_controller);
            _request.Set(output);
            return DoNext.Continue;
        }
    }
}