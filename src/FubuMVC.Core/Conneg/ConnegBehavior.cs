using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Conneg
{
    // See StoryTeller suite for Conneg
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

    public class ConnegInputHandler<T> : IConnegInputHandler where T : class
    {
        private readonly IMediaProcessor<T> _processor;

        public ConnegInputHandler(IMediaProcessor<T> processor)
        {
            _processor = processor;
        }

        public void ReadInput(CurrentRequest currentRequest, IFubuRequest request)
        {
            var input = _processor.Retrieve(currentRequest);
            request.Set(input);
        }
    }

    public class ConnegOutputHandler<T> : IConnegOutputHandler where T : class
    {
        private readonly IMediaProcessor<T> _processor;

        public ConnegOutputHandler(IMediaProcessor<T> processor)
        {
            _processor = processor;
        }

        public void WriteOutput(CurrentRequest currentRequest, IFubuRequest request)
        {
            var output = request.Get<T>();
            _processor.Write(output, currentRequest);
        }
    }

    public class NulloConnegHandler : IConnegInputHandler, IConnegOutputHandler
    {
        public void ReadInput(CurrentRequest currentRequest, IFubuRequest request)
        {
            // do nothing
        }

        public void WriteOutput(CurrentRequest currentRequest, IFubuRequest request)
        {
            // do nothing
        }
    }

    public interface IConnegOutputHandler
    {
        void WriteOutput(CurrentRequest currentRequest, IFubuRequest request);
    }

    public interface IConnegInputHandler
    {
        void ReadInput(CurrentRequest currentRequest, IFubuRequest request);
    }
}