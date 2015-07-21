using System;
using FubuCore;
using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus.Logging
{
    public class ChainExecutionStarted : MessageLogRecord
    {
        public Guid ChainId { get; set; }
        public EnvelopeToken Envelope { get; set; }

        public override string ToString()
        {
            return "Chain execution started for chain {0} / envelope {1}".ToFormat(ChainId, Envelope);
        }

        public override MessageRecord ToRecord()
        {
            return new MessageRecord(Envelope)
            {
                Message = "Chain execution started"
            };
        }
    }
}