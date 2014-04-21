using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;

namespace FubuMVC.Core.Diagnostics.Runtime
{
    public class BehaviorStart : LogRecord, DescribesItself
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

        public bool Equals(BehaviorStart other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._correlation, _correlation);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BehaviorStart)) return false;
            return Equals((BehaviorStart) obj);
        }

        public override int GetHashCode()
        {
            return (_correlation != null ? _correlation.GetHashCode() : 0);
        }

        public void Describe(Description description)
        {
            var inner = Description.For(Correlation.Node);

            description.Title = "Starting " + inner.Title;
        }
    }
}