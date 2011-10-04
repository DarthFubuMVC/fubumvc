using System;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class BehaviorTracer : IActionBehavior
    {
        private readonly IDebugReport _report;
        private readonly IDebugDetector _debugDetector;

        public BehaviorTracer(Guid chainId, IDebugReport report, IDebugDetector debugDetector)
        {
            _report = report;
            _debugDetector = debugDetector;

            _report.BehaviorId = chainId;
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
            report.BehaviorId = _report.BehaviorId;

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