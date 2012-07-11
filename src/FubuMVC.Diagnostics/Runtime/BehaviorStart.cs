using FubuMVC.Diagnostics.Runtime.Tracing;

namespace FubuMVC.Diagnostics.Runtime
{
    public class BehaviorStart
    {
        private readonly BehaviorCorrelation _correlation;

        public BehaviorStart(BehaviorCorrelation correlation)
        {
            _correlation = correlation;
        }

        public BehaviorCorrelation Correlation
        {
            get { return _correlation; }
        }
    }
}