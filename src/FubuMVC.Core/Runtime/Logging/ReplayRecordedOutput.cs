using FubuMVC.Core.Caching;

namespace FubuMVC.Core.Runtime.Logging
{
    public class ReplayRecordedOutput : LogRecord
    {
        private readonly IRecordedOutput _output;

        public ReplayRecordedOutput(IRecordedOutput output)
        {
            _output = output;
        }

        public bool Equals(ReplayRecordedOutput other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._output, _output);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ReplayRecordedOutput)) return false;
            return Equals((ReplayRecordedOutput) obj);
        }

        public override int GetHashCode()
        {
            return (_output != null ? _output.GetHashCode() : 0);
        }
    }
}