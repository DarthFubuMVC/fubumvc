using System;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Rest.Conneg
{
    // See StoryTeller suite for Conneg
    [MarkedForTermination]
    public class ConnegBehavior : IActionBehavior
    {
        private readonly IFubuRequest _request;
        private readonly IActionBehavior _inner;
        private readonly IConnegInputHandler _inputHandler;
        private readonly IConnegOutputHandler _outputHandler;

        public ConnegBehavior(IFubuRequest request, IActionBehavior inner, IConnegInputHandler inputHandler, IConnegOutputHandler outputHandler)
        {
            _request = request;
            _inner = inner;
            _inputHandler = inputHandler;
            _outputHandler = outputHandler;
        }

        private void execute(Action innerInvocation)
        {
            var currentRequest = _request.Get<CurrentRequest>();
            _inputHandler.ReadInput(currentRequest, _request);

            innerInvocation();

            _outputHandler.WriteOutput(currentRequest, _request);
        }

        public IConnegInputHandler InputHandler
        {
            get { return _inputHandler; }
        }

        public IConnegOutputHandler OutputHandler
        {
            get { return _outputHandler; }
        }

        public void Invoke()
        {
            execute(() => _inner.Invoke());
        }

        public void InvokePartial()
        {
            execute(() => _inner.InvokePartial());
        }
    }
}