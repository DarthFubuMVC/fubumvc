using System;
using FubuCore.Logging;

namespace FubuMVC.Core.ServiceBus.Polling
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