using System;
using FubuCore.Logging;

namespace FubuTransportation.Polling
{
    public class PollingJobStarted : LogRecord
    {
        public string Description { get; set; }
        public Guid JobRun { get; set; }


        public override string ToString()
        {
            return "PollingJob Started: " + Description;
        }
    }
}