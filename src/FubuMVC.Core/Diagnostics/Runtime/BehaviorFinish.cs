using System;
using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using ExceptionReport = FubuCore.Logging.ExceptionReport;

namespace FubuMVC.Core.Diagnostics.Runtime
{
    public class BehaviorFinish : LogRecord, DescribesItself
    {
        private readonly BehaviorCorrelation _correlation;
        private bool _succeeded = true;
        private ExceptionReport _exception;

        public BehaviorFinish(BehaviorCorrelation correlation)
        {
            _correlation = correlation;
        }

        public BehaviorCorrelation Correlation
        {
            get { return _correlation; }
        }

        public bool Succeeded
        {
            get {
                return _succeeded;
            }
        }

        public ExceptionReport Exception
        {
            get {
                return _exception;
            }
        }

        public void LogException(Exception ex)
        {
            _exception = new ExceptionReport(ex){
                CorrelationId = _correlation.Node.UniqueId
            };

            _succeeded = false;
        }

        public bool Equals(BehaviorFinish other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._correlation, _correlation);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BehaviorFinish)) return false;
            return Equals((BehaviorFinish) obj);
        }

        public override int GetHashCode()
        {
            return (_correlation != null ? _correlation.GetHashCode() : 0);
        }

        public void Describe(Description description)
        {
            var inner = Description.For(Correlation.Node);

            description.Title = "Finished " + inner.Title;
        }
    }
}