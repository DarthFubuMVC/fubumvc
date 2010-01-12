using System;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class OneInOneOutActionInvoker<TController, TInput, TOutput> : BasicBehavior where TInput : class
                                                                                        where TOutput : class
    {
        private readonly Func<TController, TInput, TOutput> _action;
        private readonly TController _controller;
        private readonly IFubuRequest _request;

        public OneInOneOutActionInvoker(IFubuRequest request, TController controller,
                                        Func<TController, TInput, TOutput> action)
            : base(PartialBehavior.Executes)
        {
            _request = request;
            _controller = controller;
            _action = action;
        }

        // TODO:  Harden against failures?
        protected override DoNext performInvoke()
        {
            var input = _request.Get<TInput>();
            TOutput output = _action(_controller, input);
            _request.Set(output);

            return DoNext.Continue;
        }


    }
}