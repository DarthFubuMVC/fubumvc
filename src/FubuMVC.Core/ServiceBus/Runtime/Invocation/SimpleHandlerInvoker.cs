﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public class SimpleHandlerInvoker<TController, TInput> : BasicBehavior where TInput : class
    {
        private readonly Action<TController, TInput> _action;
        private readonly TController _controller;
        private readonly IFubuRequest _request;

        public SimpleHandlerInvoker(IFubuRequest request, TController controller,
                                    Action<TController, TInput> action)
            : base(PartialBehavior.Executes)
        {
            _request = request;
            _controller = controller;
            _action = action;
        }

        protected override Task<DoNext> performInvoke()
        {
            var input = _request.Find<TInput>().Single();
            _action(_controller, input);
            return Task.FromResult(DoNext.Continue);
        }
    }
}