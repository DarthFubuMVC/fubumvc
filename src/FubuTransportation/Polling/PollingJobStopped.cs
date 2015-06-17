using System;
using FubuCore.Logging;

namespace FubuTransportation.Polling
{
    public class PollingJobStopped : LogRecord
    {
        public Type JobType { get; set; }

        public override string ToString()
        {
            return "PollingJob Stopped: " + JobType.FullName;
        }
    }
}