using System;
using FubuCore.Logging;
using FubuMVC.Core.Diagnostics.Instrumentation;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public class DiagnosticHandlerPipeline : IHandlerPipeline
    {
        private readonly HandlerPipeline _pipeline;
        private readonly IExecutionLogger _logger;

        public DiagnosticHandlerPipeline(HandlerPipeline pipeline, IExecutionLogger logger)
        {
            _pipeline = pipeline;
            _logger = logger;
        }

        public void Invoke(Envelope envelope)
        {
            _pipeline.Invoke(envelope);
        }

        public void Receive(Envelope envelope)
        {
            // Here is where everything should happen
            throw new NotImplementedException();
        }

        public ILogger Logger
        {
            get { return _pipeline.Logger; }
        }
    }
}