using System;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class OneInZeroOutActionInvoker<TController, TInput> : BasicBehavior where TInput : class
    {
        private readonly Action<TController, TInput> _action;
        private readonly TController _controller;
        private readonly IFubuRequest _request;

        public OneInZeroOutActionInvoker(IFubuRequest request, TController controller,
                                         Action<TController, TInput> action)
            : base(PartialBehavior.Executes)
        {
            _request = request;
            _controller = controller;
            _action = action;
        }

        protected override DoNext performInvoke()
        {
            var input = _request.Get<TInput>();
            _action(_controller, input);
            return DoNext.Continue;
        }
    }
}