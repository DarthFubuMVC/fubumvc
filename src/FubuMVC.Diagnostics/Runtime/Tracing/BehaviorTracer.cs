using System;
using FubuCore.Logging;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Diagnostics.Runtime.Tracing
{
    public class BehaviorTracer : WrappingBehavior
    {
        private readonly BehaviorCorrelation _correlation;
        private readonly IDebugDetector _debugDetector;
        private readonly ILogger _logger;

        public BehaviorTracer(BehaviorCorrelation correlation, IDebugDetector debugDetector, ILogger logger)
        {
            _correlation = correlation;
            _debugDetector = debugDetector;
            _logger = logger;
        }

        protected override void invoke(Action action)
        {
            _logger.DebugMessage(() => new BehaviorStart(_correlation));

            try
            {
                action();
            }
            catch (Exception ex)
            {
                _logger.Error("Behavior Failure", ex);

                if (!_debugDetector.IsOutputWritingLatched())
                {
                    throw;
                }
            }

            _logger.DebugMessage(() => new BehaviorFinish(_correlation));
        }
    }
}