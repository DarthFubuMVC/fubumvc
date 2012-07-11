using FubuMVC.Diagnostics.Runtime.Tracing;

namespace FubuMVC.Diagnostics.Runtime
{
    public class BehaviorFinish
    {
        private readonly BehaviorCorrelation _correlation;

        public BehaviorFinish(BehaviorCorrelation correlation)
        {
            _correlation = correlation;
        }

        public BehaviorCorrelation Correlation
        {
            get { return _correlation; }
        }
    }
}