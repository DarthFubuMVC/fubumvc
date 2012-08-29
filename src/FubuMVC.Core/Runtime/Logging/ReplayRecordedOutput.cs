using System;
using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuMVC.Core.Caching;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Runtime.Logging
{
    public class ReplayRecordedOutput : LogRecord, IHaveContentType, DescribesItself
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

        public void Describe(Description description)
        {
            description.Title = "Replaying recorded output";
            description.AddList("Outputs", _output.Outputs);
        }

        public string ContentType
        {
            get { return _output.ContentType; }
        }
    }
}