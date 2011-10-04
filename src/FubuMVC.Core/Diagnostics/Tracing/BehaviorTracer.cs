using System;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class BehaviorTracer : IActionBehavior
    {
        private readonly IDebugReport _report;
        private readonly IDebugDetector _debugDetector;
        private readonly Guid _behaviorId;

        public BehaviorTracer(BehaviorCorrelation correlation, IDebugReport report, IDebugDetector debugDetector)
        {
            _report = report;
            _debugDetector = debugDetector;

            if (_report.BehaviorId == Guid.Empty)
            {
                _report.BehaviorId = correlation.ChainId;
            }

            _behaviorId = correlation.BehaviorId;
        }

        public IActionBehavior Inner { get; set; }

        public void Invoke()
        {
            invoke(() => Inner.Invoke());
        }

        public void InvokePartial()
        {
            invoke(() => Inner.InvokePartial());
        }

        private void invoke(Action action)
        {
            var report = _report.StartBehavior(Inner);
            report.BehaviorId = _behaviorId;

            try
            {
                action();
            }
            catch (Exception ex)
            {
                _report.MarkException(ex);
                if (!_debugDetector.IsOutputWritingLatched())
                {
                    throw;
                }
            }

            _report.EndBehavior();
        }
    }
}