using System;
using System.Diagnostics;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Logging;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public class ChainExecutionWatcher : IDisposable
    {
        private readonly ILogger _logger;
        private readonly HandlerChain _chain;
        private readonly Envelope _envelope;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public ChainExecutionWatcher(ILogger logger, HandlerChain chain, Envelope envelope)
        {
            _logger = logger;
            _chain = chain;
            _envelope = envelope;

            _logger.DebugMessage(() => new ChainExecutionStarted
            {
                ChainId = chain.UniqueId,
                Envelope = envelope.ToToken()
            });

            _stopwatch.Start();
        }

        public void Dispose()
        {
            _stopwatch.Stop();

            _logger.DebugMessage(() => new ChainExecutionFinished
            {
                ChainId = _chain.UniqueId,
                ElapsedMilliseconds = _stopwatch.ElapsedMilliseconds,
                Envelope = _envelope.ToToken()
            });
        }
    }
}