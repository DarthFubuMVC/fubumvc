using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Runtime
{
    public class DiagnosticPartialFactory : IPartialFactory
    {
        private readonly IChainExecutionLog _log;
        private readonly PartialFactory _factory;

        public DiagnosticPartialFactory(IChainExecutionLog log, PartialFactory factory)
        {
            _log = log;
            _factory = factory;
        }

        public IActionBehavior BuildBehavior(BehaviorChain chain)
        {
            var behavior = _factory.BuildBehavior(chain);
            return new PartialLoggingBehavior(_log, chain, behavior);
        }

        public IActionBehavior BuildPartial(BehaviorChain chain)
        {
            var behavior = _factory.BuildPartial(chain);
            return new PartialLoggingBehavior(_log, chain, behavior);
        }

        public class PartialLoggingBehavior : WrappingBehavior
        {
            private readonly IChainExecutionLog _log;
            private readonly BehaviorChain _chain;

            public PartialLoggingBehavior(IChainExecutionLog log, BehaviorChain chain, IActionBehavior inner) 
            {
                _log = log;
                _chain = chain;
                Inner = inner;
            }

            protected override void invoke(Action action)
            {
                _log.StartSubject(_chain);
                try
                {
                    action();
                }
                finally
                {
                    _log.FinishSubject();
                }
            }
        }
    }
}