using System;
using FubuCore.Logging;

namespace FubuTransportation.Polling
{
    public class PollingJobFailed : LogRecord
    {
        public string Description { get; set; }
        public Exception Exception { get; set; }
        public Guid JobRun { get; set; }
    }
}