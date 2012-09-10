using System;
using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuMVC.Core.Caching;

namespace FubuMVC.Core.Runtime.Logging
{
    [Title("Finished recording output")]
    public class FinishedRecordingOutput : LogRecord, DescribesItself
    {
        private readonly RecordedOutput _output;

        public FinishedRecordingOutput(RecordedOutput output)
        {
            _output = output;
        }

        public RecordedOutput Output
        {
            get { return _output; }
        }

        public void Describe(Description description)
        {
            description.AddList("Writes", _output.Outputs);
        }
    }
}